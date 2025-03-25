using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Company;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlorriJob.WebAPI.Controllers
{
	[Route("api/v1/[controller]/[action]")]
	[ApiController]
	public class CompaniesController : ControllerBase
	{
		private ICompanyService _companyService { get; }
        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
		[HttpGet("all")]
		public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 1, bool isPaginated = true)
		{
			var response = await _companyService.GetAllAsync(pageNumber, pageSize, isPaginated);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var response = await _companyService.GetByIdAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpGet("search")]
		public async Task<IActionResult> SearchByName([FromQuery] string name, int pageNumber = 1, int pageSize = 1, bool isPaginated = true)
		{
			var response = await _companyService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpPost]
		public async Task<IActionResult> Create([FromForm] CompanyCreateDto companyCreateDto) 
		{
			var response = await _companyService.CreateAsync(companyCreateDto);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromForm] CompanyUpdateDto companyUpdateDto)
		{
			var response = await _companyService.UpdateAsync(id, companyUpdateDto);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var response = await _companyService.DeleteAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}
    }
}
