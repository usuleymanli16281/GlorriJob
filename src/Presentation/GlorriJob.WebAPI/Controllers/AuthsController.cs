using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Identity;
using GlorriJob.Domain;
using Microsoft.AspNetCore.Mvc;

namespace GlorriJob.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthsController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthsController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("connect/token")]
    public async Task<IActionResult> RefreshToken(string refreshtoken)
    {
        var response = await _authService.RefreshToken(refreshtoken);
        return StatusCode((int)response.StatusCode, response);
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);
		    return StatusCode((int)response.StatusCode, response);
	}


    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var response = await _authService.RegisterAsync(registerDto);
		    return StatusCode((int)response.StatusCode, response);
	}
    [HttpGet("[action]")]
    public IActionResult GetEmailFromToken([FromQuery] string token)
    {
        var response = _authService.GetEmailFromToken(token);
        return StatusCode((int)response.StatusCode, response);
    }
}
