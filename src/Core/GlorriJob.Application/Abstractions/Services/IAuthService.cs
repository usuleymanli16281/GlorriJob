namespace GlorriJob.Application.Abstractions.Services;

public interface IAuthService
{
    string Login(string username, string password);
    bool ValidateToken(string token);
    string RefreshToken(string refreshToken);
}
