﻿using AutoMapper;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Identity;
using GlorriJob.Application.Validations.City;
using GlorriJob.Application.Validations.Identity;
using GlorriJob.Common.Contracts;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using GlorriJob.Persistence.Contexts;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace GlorriJob.Persistence.Implementations.Services;

public class AuthService : IAuthService
{
	private IJwtService _jwtService { get; }
	private UserManager<User> _userManager { get; }
	private IMapper _mapper { get; }
	private IConfiguration _configuration { get; }
	private IOtpCacheService _cacheService { get; }
	private IPublishEndpoint _publishEndpoint { get; }
	public AuthService(IJwtService jwtService,
		UserManager<User> userManager,
		IMapper mapper,
		IConfiguration configuration,
		IOtpCacheService cacheService,
		IPublishEndpoint publishEndpoint)
	{
		_jwtService = jwtService;
		_userManager = userManager;
		_mapper = mapper;
		_configuration = configuration;
		_cacheService = cacheService;
		_publishEndpoint = publishEndpoint;
	}
	public async Task<BaseResponse<object>> RefreshTokenAsync(string refreshtoken)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshtoken);
		if (user is null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "This refreshToken is not valid",
				Data = null
			};
		}
		if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "This refreshToken is expired",
				Data = null
			};
		}
		var claims = new List<Claim>();
		claims.AddRange(
		[
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Name),
			new Claim(ClaimTypes.Surname, user.Surname ?? ""),
			new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
		]);
		var roles = await _userManager.GetRolesAsync(user);
		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
		string token = _jwtService.GenerateAccessToken(claims);
		_ = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationHours"], out int accessTokenExpiryTime);
		string refreshToken = _jwtService.GenerateRefreshToken();
		_ = int.TryParse(_configuration["JwtSettings:RefreshTokenExpirationHours"], out int refreshTokenExpiryTime);
		user.RefreshToken = refreshToken;
		user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(refreshTokenExpiryTime);
		await _userManager.UpdateAsync(user);
		return new BaseResponse<object>
		{
			StatusCode = HttpStatusCode.OK,
			Message = "New tokens are created",
			Data = new
			{
				AccessToken = token,
				AccessTokenExpirationHours = accessTokenExpiryTime,
				RefreshToken = refreshToken,
				RefreshTokenExpirationHours = refreshTokenExpiryTime
			}
		};

	}
	public async Task<BaseResponse<object>> LoginAsync(LoginDto loginDto)
	{
		var validator = new LoginDtoValidator();
		var validationResult = await validator.ValidateAsync(loginDto);
		if (!validationResult.IsValid)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = string.Join(";", validationResult.Errors.Select(e => e.ErrorMessage)),
				Data = null
			};
		}
		var user = await _userManager.Users.FirstOrDefaultAsync(u => u.EmailConfirmed && u.Email == loginDto.Email);
		if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "Invalid email or password."
			};
		}
		var claims = new List<Claim>();
		claims.AddRange(
		[
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Name),
			new Claim(ClaimTypes.Surname, user.Surname ?? ""),
			new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
		]);
		var roles = await _userManager.GetRolesAsync(user);
		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
		string token = _jwtService.GenerateAccessToken(claims);
		_ = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationHours"], out int accessTokenExpiryTime);
		string refreshToken = _jwtService.GenerateRefreshToken();
		_ = int.TryParse(_configuration["JwtSettings:RefreshTokenExpirationHours"], out int refreshTokenExpiryTime);
		user.RefreshToken = refreshToken;
		user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(refreshTokenExpiryTime);
		await _userManager.UpdateAsync(user);
		return new BaseResponse<object>
		{
			StatusCode = HttpStatusCode.OK,
			Message = "User successfully logged in",
			Data = new
			{
				AccessToken = token,
				AccessTokenExpirationHours = accessTokenExpiryTime,
				RefreshToken = refreshToken,
				RefreshTokenExpirationHours = refreshTokenExpiryTime
			}
		};

	}

	public async Task<BaseResponse<object>> RegisterAsync(RegisterDto registerDto)
	{
		var validator = new RegisterDtoValidator();
		var validationResult = await validator.ValidateAsync(registerDto);
		if (!validationResult.IsValid)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = string.Join(";", validationResult.Errors.Select(e => e.ErrorMessage))
			};
		}
		var registeredUser = await _userManager.FindByEmailAsync(registerDto.Email);
		if (registeredUser is not null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "This email already registered."
			};
		}
		User user = _mapper.Map<User>(registerDto);
		user.UserName = registerDto.Email;
		user.SecurityStamp = Guid.NewGuid().ToString().Replace("-", "");
		var result = await _userManager.CreateAsync(user, registerDto.Password);
		if (!result.Succeeded)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = string.Join("; ", result.Errors.Select(e => e.Description))
			};
		}
		var otp = new Random().Next(100000, 999999).ToString();
		await _cacheService.SetOtpAsync(registerDto.Email, otp);
		await _publishEndpoint.Publish(new SendEmailMessage
		{
			To = registerDto.Email,
			Subject = "User Verification",
			Body = $"This is your otp: {otp}"
		});
		BackgroundJob.Schedule(() => DeleteUnconfirmedUserAsync(user.Id.ToString()), TimeSpan.FromMinutes(15));
		return new BaseResponse<object>
		{
			Message = "User registered successfully. Otp has been sent",
			StatusCode = HttpStatusCode.Created
		};
	}
	public async Task<BaseResponse<object>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(u => u.EmailConfirmed && u.Email == forgotPasswordDto.Email);
		if (user is null)
		{
			return new BaseResponse<object>
			{
				Message = "User is not found.",
				StatusCode = HttpStatusCode.NotFound,
			};
		}

		var otp = new Random().Next(100000, 999999).ToString();
		await _cacheService.SetOtpAsync(forgotPasswordDto.Email, otp);

		await _publishEndpoint.Publish(new SendEmailMessage
		{
			To = forgotPasswordDto.Email,
			Body = $"This is your otp {otp}",
			Subject = "User Verification for the new password"
		});

		return new BaseResponse<object>
		{
			Message = "OTP has been sent to your email.",
			StatusCode = HttpStatusCode.OK
		};
	}
	public async Task<BaseResponse<object>> VerifyUserAsync(VerifyUserDto verifyUserDto)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == verifyUserDto.Email);
		if (user is null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "User is not found."
			};
		}

		if (user.EmailConfirmed)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.Conflict,
				Message = "User is already verified."
			};
		}

		var cachedOtp = await _cacheService.GetOtpAsync(verifyUserDto.Email);
		if (cachedOtp is null || cachedOtp != verifyUserDto.Otp)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "Invalid or expired OTP."
			};
		}

		user.EmailConfirmed = true;
		await _userManager.UpdateAsync(user);
		await _userManager.AddToRoleAsync(user, "user");
		await _cacheService.RemoveOtpAsync(verifyUserDto.Email);

		return new BaseResponse<object>
		{
			StatusCode = HttpStatusCode.NoContent,
			Message = "Email is  successfully confirmed."
		};
	}
	public async Task<BaseResponse<object>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(u => u.EmailConfirmed && u.Email == resetPasswordDto.Email);
		if (user is null)
		{
			return new BaseResponse<object>
			{
				Message = "User is not found.",
				StatusCode = HttpStatusCode.NotFound
			};
		}

		var cachedOtp = await _cacheService.GetOtpAsync(resetPasswordDto.Email);
		if (cachedOtp is null || cachedOtp != resetPasswordDto.Otp)
		{
			return new BaseResponse<object>
			{
				Message = "Invalid or expired OTP.",
				StatusCode = HttpStatusCode.BadRequest
			};
		}

		var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
		var result = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordDto.NewPassword);
		if (!result.Succeeded)
		{
			return new BaseResponse<object>
			{
				Message = string.Join("; ", result.Errors.Select(e => e.Description)),
				StatusCode = HttpStatusCode.BadRequest,
			};
		}

		await _cacheService.RemoveOtpAsync(resetPasswordDto.Email);

		return new BaseResponse<object>
		{
			Message = "Password has been successfully reset.",
			StatusCode = HttpStatusCode.OK
		};
	}
	public async Task DeleteUnconfirmedUserAsync(string userId)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user is not null && !user.EmailConfirmed)
		{
			await _userManager.DeleteAsync(user);
		}
	}
}

