using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Identity;
using GlorriJob.Domain;
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
        var response = await _authService.RefreshToken(refreshtoken);
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
}
