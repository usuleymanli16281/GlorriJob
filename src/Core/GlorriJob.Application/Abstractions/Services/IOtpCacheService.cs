

namespace GlorriJob.Application.Abstractions.Services
{
	public interface IOtpCacheService
	{
		Task SetOtpAsync(string email, string otp);
		Task<string?> GetOtpAsync(string email);
		Task RemoveOtpAsync(string email);
	}
}
