using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Biography;
using GlorriJob.Application.Dtos.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlorriJob.WebAPI.Controllers
{
	[Route("api/v1/[controller]/[action]")]
	[ApiController]
	public class BiographiesController : ControllerBase
	{
		private IBiographyService _biographyService{ get; }
		public BiographiesController(IBiographyService biographyService)
		{
			_biographyService = biographyService;
		}
		[HttpGet("all")]
		[Authorize(Policy = "UserPolicy")]
		public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 1, bool isPaginated = true)
		{
			var response = await _biographyService.GetAllAsync(pageNumber, pageSize, isPaginated);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var response = await _biographyService.GetByIdAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpPost]
		public async Task<IActionResult> Create([FromForm] BiographyCreateDto biographyCreateDto)
		{
			var response = await _biographyService.CreateAsync(biographyCreateDto);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromForm] BiographyUpdateDto biographyUpdateDto)
		{
			var response = await _biographyService.UpdateAsync(id, biographyUpdateDto);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var response = await _biographyService.DeleteAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}
