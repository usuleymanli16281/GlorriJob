using GlorriJob.Application.Dtos.IdentityDtos;
using GlorriJob.Common.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface IAuthService
{
    Task<BaseResponse<object>> RegisterAsync(RegisterDto registerDto);
    Task<BaseResponse<object>> LoginAsync(LoginDto loginDto);
}
