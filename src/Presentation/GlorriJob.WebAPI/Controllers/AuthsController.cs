﻿using GlorriJob.Application.Abstractions.Services;
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);
        if (response.CustomStatusCode == ResponseStatusCode.Success.ToString())
        {
            return Ok(response);
        }
        return Unauthorized(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var response = await _authService.RegisterAsync(registerDto);
        if (response.CustomStatusCode == ResponseStatusCode.Created.ToString())
        {
            return CreatedAtAction(nameof(Register), new { username = registerDto.Username }, response);
        }
        return BadRequest(response);
    }
}
