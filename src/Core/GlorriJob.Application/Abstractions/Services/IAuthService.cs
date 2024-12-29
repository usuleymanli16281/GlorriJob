using GlorriJob.Application.Dtos.Identity;
using GlorriJob.Common.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface IAuthService
{
    Task<ApiResponse<object>> RegisterAsync(RegisterDto registerDto);
    Task<ApiResponse<object>> LoginAsync(LoginDto loginDto);
}
