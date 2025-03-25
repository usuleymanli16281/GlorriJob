using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Common.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GlorriJob.WebAPI.Controllers
{
	[Route("api/v1/[controller]/[action]")]
	[ApiController]
	public class RolesController : ControllerBase
	{
        private readonly IRoleService _roleService;
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
		}
		[HttpPost]
		public async Task<IActionResult> CreateRoleAsync(string roleName)
		{
			var response = await _roleService.CreateRoleAsync(roleName);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpGet]
		public async Task<IActionResult> RoleExistsAsync(string roleName)
		{
			var response = await _roleService.RoleExistsAsync(roleName);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpGet]
		public async Task<IActionResult> GetAllRolesAsync()
		{
			var response = await _roleService.GetAllRolesAsync();
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpDelete]
		public async Task<IActionResult> DeleteRoleAsync(string roleName)
		{
			var response = await _roleService.DeleteRoleAsync(roleName);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpPost]
		public async Task<IActionResult> AddUserToRoleAsync(string userId, string roleName)
		{
			var response = await _roleService.AddUserToRoleAsync(userId, roleName);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpPost]
		public async Task<IActionResult> RemoveUserFromRoleAsync(string userId, string roleName)
		{
			var response = await _roleService.RemoveUserFromRoleAsync(userId, roleName);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}
