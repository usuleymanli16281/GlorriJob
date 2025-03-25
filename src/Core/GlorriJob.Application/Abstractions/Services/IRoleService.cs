using GlorriJob.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Abstractions.Services
{
	public interface IRoleService
	{
		Task<BaseResponse<object>> CreateRoleAsync(string roleName);
		Task<BaseResponse<bool>> RoleExistsAsync(string roleName);
		Task<BaseResponse<List<string>>> GetAllRolesAsync();
		Task<BaseResponse<object>> DeleteRoleAsync(string roleName);
		Task<BaseResponse<object>> AddUserToRoleAsync(string userId, string roleName);
		Task<BaseResponse<object>> RemoveUserFromRoleAsync(string userId, string roleName);
	}
}
