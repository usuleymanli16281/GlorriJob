using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Common.Shared;
using Microsoft.AspNetCore.Authorization;
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
		public async Task<IActionResult> CreateRole(string roleName)
		{
			var response = await _roleService.CreateRoleAsync(roleName);
			return StatusCode((int)response.StatusCode, response);
		}

		[HttpGet]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> GetAllRoles()
		{
			var response = await _roleService.GetAllRolesAsync();
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpDelete]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> DeleteRole(string roleName)
		{
			var response = await _roleService.DeleteRoleAsync(roleName);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpPost]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> AddUserToRole(string userId, string roleName)
		{
			var response = await _roleService.AddUserToRoleAsync(userId, roleName);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpPost]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
		{
			var response = await _roleService.RemoveUserFromRoleAsync(userId, roleName);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}
