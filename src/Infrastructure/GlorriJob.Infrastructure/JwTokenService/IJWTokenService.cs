using System.Security.Claims;

namespace GlorriJob.Infrastructure.JwTokenService;

public interface IJWTokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}
