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
        throw new NotImplementedException();
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
            UserName = registerDto.Email,
            SecurityStamp = Guid.NewGuid().ToString().Replace("-","")
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
            Data = new { Name = user.Name,  Surname = user.Surname, Email = user.Email }
        };
    }
}

