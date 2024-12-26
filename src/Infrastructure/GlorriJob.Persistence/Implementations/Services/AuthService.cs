using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Identity;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GlorriJob.Persistence.Implementations.Services;

public class AuthService : IAuthService
{
    private IJwtService _jwtService { get; }
    private UserManager<User> _userManager { get; }
    private Dictionary<string, string> _refreshTokens { get; }
    public AuthService(IJwtService jwtService, UserManager<User> userManager)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _refreshTokens = new Dictionary<string, string>();

    }
    public async Task<BaseResponse<object>> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return new BaseResponse<object>
            {
                StatusCode = "401",
                Message = "Invalid username or password.",
                Data = null
            };
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = _jwtService.GenerateRefreshToken();

        _refreshTokens[user.UserName!] = refreshToken;

        return new BaseResponse<object>
        {
            StatusCode = "200",
            Message = "Login is successful.",
            Data = new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

    public async Task<BaseResponse<object>> RegisterAsync(RegisterDto registerDto)
    {
        var registeredUser = await _userManager.FindByNameAsync(registerDto.Email);
        if (registeredUser is not null)
        {
            return new BaseResponse<object>
            {
                StatusCode = "400",
                Message = "This email already registered.",
                Data = null
            };
        }

        var user = new User
        {
            Name = registerDto.Name,
            Surname = registerDto.Surname,
            Email = registerDto.Email,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            return new BaseResponse<object>
            {
                StatusCode = "400",
                Message = string.Join("; ", result.Errors.Select(e => e.Description)),
                Data = null
            };
        }

        return new BaseResponse<object>
        {
            StatusCode = "201",
            Message = "User registered successfully.",
            Data = new { Username = user.UserName, Email = user.Email }
        };
    }
}

