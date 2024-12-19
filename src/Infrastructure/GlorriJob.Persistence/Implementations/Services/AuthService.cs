using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.IdentityDtos;
using GlorriJob.Persistence.Exceptions;
using Microsoft.Extensions.Configuration;

namespace GlorriJob.Persistence.Implementations.Services;

public class AuthService : IAuthService
{
    private IAuthService _authService { get; }
    public AuthService(IAuthService authService)
    {
        _authService = authService;
    }
    public string Login(string username, string password)
    {
        throw new NotImplementedException();
    }

    public string RefreshToken(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public bool ValidateToken(string token)
    {
        throw new NotImplementedException();
    }
}
