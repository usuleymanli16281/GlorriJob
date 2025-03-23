using GlorriJob.Application.Abstractions.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Infrastructure.Services
{
	public class OtpCacheService : IOtpCacheService
	{
		private readonly IDatabase _cache;
		private readonly TimeSpan _expiryTime = TimeSpan.FromMinutes(3);

		public OtpCacheService(IConnectionMultiplexer redis)
		{
			_cache = redis.GetDatabase();
		}
		public async Task<string?> GetOtpAsync(string email)
		{
			return await _cache.StringGetAsync(email);
		}

		public async Task RemoveOtpAsync(string email)
		{
			await _cache.KeyDeleteAsync(email);
		}

		public async Task SetOtpAsync(string email, string otp)
		{
			await _cache.StringSetAsync(email, otp, _expiryTime);
		}
	}
}
