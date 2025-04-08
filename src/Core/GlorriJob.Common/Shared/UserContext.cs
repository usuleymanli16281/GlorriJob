using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Common.Shared
{
	public class UserContext
	{
		private static IHttpContextAccessor? _httpContextAccessor { get; set; }

        public static void Configure(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public static string? GetUserId()
		{
			if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated != true)
				return null;

			return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}

		public static string? GetUserEmail()
		{
			if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated != true)
				return null;

			return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
		}
	}
}
