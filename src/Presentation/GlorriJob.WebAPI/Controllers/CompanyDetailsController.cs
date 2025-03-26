using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.CompanyDetail;
using GlorriJob.Application.Dtos.VacancyDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlorriJob.WebAPI.Controllers
{
	[Route("api/v1/[controller]/[action]")]
	[ApiController]
	public class CompanyDetailsController : ControllerBase
	{
		private readonly ICompanyDetailService _companyDetailService;
		public CompanyDetailsController(ICompanyDetailService companyDetailService)
		{
			_companyDetailService = companyDetailService;
		}

		[HttpPost]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> Create(CompanyDetailCreateDto companyDetailCreateDto)
		{
			var response = await _companyDetailService.CreateAsync(companyDetailCreateDto);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpDelete]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var response = await _companyDetailService.DeleteAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpGet]
		[Authorize(Policy = "UserPolicy")]
		public async Task<IActionResult> GetByCompanyId(Guid companyId)
		{
			var response = await _companyDetailService.GetByCompanyIdAsync(companyId);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpGet("{id}")]
		[Authorize(Policy = "UserPolicy")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var response = await _companyDetailService.GetByIdAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}
		[HttpPut]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> Update(Guid id, CompanyDetailUpdateDto companyDetailUpdateDto)
		{
			var response = await _companyDetailService.UpdateAsync(id, companyDetailUpdateDto);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}
