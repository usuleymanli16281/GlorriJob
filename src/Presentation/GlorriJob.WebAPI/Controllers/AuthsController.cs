using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlorriJob.WebAPI.Controllers;

[Route("api/v1/[controller]/[action]")]
[ApiController]
public class AuthsController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthsController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost]
    public async Task<IActionResult> RefreshToken(string refreshtoken)
    {
        var response = await _authService.RefreshTokenAsync(refreshtoken);
        return StatusCode((int)response.StatusCode, response);
    }


    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);
		    return StatusCode((int)response.StatusCode, response);
	}


    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var response = await _authService.RegisterAsync(registerDto);
		    return StatusCode((int)response.StatusCode, response);
	}
	[HttpPost]
	public async Task<IActionResult> VerifyUser([FromBody] VerifyUserDto verifyUserDto)
	{
		var response = await _authService.VerifyUserAsync(verifyUserDto);
		return StatusCode((int)response.StatusCode, response);
	}
	[HttpPost]
	public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
	{
		var response = await _authService.ForgotPasswordAsync(forgotPasswordDto);
		return StatusCode((int)response.StatusCode, response);
	}

	[HttpPost]
	public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
	{
		var response = await _authService.ResetPasswordAsync(resetPasswordDto);
		return StatusCode((int)response.StatusCode, response);
	}
}
