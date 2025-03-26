using System.Security.Claims;

namespace GlorriJob.Application.Abstractions.Services;

public interface IJwtService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}
