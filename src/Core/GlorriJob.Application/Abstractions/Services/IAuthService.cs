using GlorriJob.Application.Dtos.Identity;
using GlorriJob.Common.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface IAuthService
{
	Task<BaseResponse<object>> RegisterAsync(RegisterDto registerDto);
	Task<BaseResponse<object>> LoginAsync(LoginDto loginDto);
	Task<BaseResponse<object>> RefreshTokenAsync(string refreshtoken);
	Task<BaseResponse<object>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
	Task<BaseResponse<object>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
	Task<BaseResponse<object>> VerifyUserAsync(VerifyUserDto verifyUserDto);
}
