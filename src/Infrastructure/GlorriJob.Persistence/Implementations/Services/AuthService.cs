using AutoMapper;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Identity;
using GlorriJob.Application.Validations.City;
using GlorriJob.Application.Validations.Identity;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using GlorriJob.Persistence.Contexts;
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
	private GlorriJobDbContext _context { get; }
	public AuthService(IJwtService jwtService,
		UserManager<User> userManager,
		IMapper mapper,
		IConfiguration configuration,
		GlorriJobDbContext context)
	{
		_jwtService = jwtService;
		_userManager = userManager;
		_mapper = mapper;
		_configuration = configuration;
		_context = context;
	}
	public async Task<BaseResponse<object>> RefreshToken(string refreshtoken)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken==refreshtoken);
		if(user is null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "This refreshToken is not valid",
				Data = null
			};
		}
		if(user.RefreshTokenExpiryTime < DateTime.UtcNow)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "This refreshToken is expired",
				Data = null
			};
		}
		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
			new Claim(ClaimTypes.Name,user.Name),
			new Claim(ClaimTypes.Surname,user.Surname ?? ""),
			new Claim(ClaimTypes.Email, user.Email!)
		};
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
				AccessTokenExpiration = accessTokenExpiryTime,
				RefreshToken = refreshToken,
				RefreshTokenExpiration = refreshTokenExpiryTime
			}
		};

	}
	public async Task<BaseResponse<object>> LoginAsync(LoginDto loginDto)
	{
		var validator = new LoginDtoValidator();
		var validationResult = await validator.ValidateAsync(loginDto);
		if(!validationResult.IsValid)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = string.Join(";",validationResult.Errors.Select(e => e.ErrorMessage)),
				Data = null
			};
		}
		User? user = await _userManager.FindByEmailAsync(loginDto.Email);
		var checkedPassword = false;
		if (user is not null)
		{
			checkedPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
		}
		if (user is null || !checkedPassword)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "Email or password is wrong.",
				Data = null
			};
		}
		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name,user.Name),
			new Claim(ClaimTypes.Surname,user.Surname ?? ""),
			new Claim(ClaimTypes.Email, loginDto.Email)
		};
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
				AccessTokenExpiration = accessTokenExpiryTime,
				RefreshToken = refreshToken,
				RefreshTokenExpiration = refreshTokenExpiryTime
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
				Message = string.Join(";", validationResult.Errors.Select(e => e.ErrorMessage)),
				Data = null
			};
		}
		var registeredUser = await _userManager.FindByNameAsync(registerDto.Email);
		if (registeredUser is not null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "This email already registered.",
				Data = null
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
				Message = string.Join("; ", result.Errors.Select(e => e.Description)),
				Data = null
			};
		}

		return new BaseResponse<object>
		{
			StatusCode = HttpStatusCode.Created,
			Message = "User registered successfully.",
			Data = new { Name = user.Name, Surname = user.Surname, Email = user.Email }
		};
	}
}

