using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Identity;
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
        return StatusCode(int.Parse(response.StatusCode!), response);
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);
		return StatusCode(int.Parse(response.StatusCode!), response);
	}

    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var response = await _authService.RegisterAsync(registerDto);
        if (response.StatusCode == "201")
        {
            return CreatedAtAction(nameof(Register), response.Data);
        }
        return BadRequest(response);
    }
}
