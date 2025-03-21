using GlorriJob.Application.Dtos.Identity;
using GlorriJob.Common.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface IAuthService
{
	Task<BaseResponse<object>> RegisterAsync(RegisterDto registerDto);
	Task<BaseResponse<object>> LoginAsync(LoginDto loginDto);
	Task<BaseResponse<object>> RefreshToken(string refreshtoken);
	Task<BaseResponse<object>> VerifyUserAsync(VerifyUserDto verifyUserDto);
}
