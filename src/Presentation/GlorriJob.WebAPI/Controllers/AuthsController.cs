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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);
        if (response.StatusCode == "200")
        {
            return Ok(response);
        }
        return Unauthorized(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var response = await _authService.RegisterAsync(registerDto);
        if (response.StatusCode == "201")
        {
            return CreatedAtAction(nameof(Register), new { username = registerDto.Username }, response);
        }
        return BadRequest(response);
    }
}
