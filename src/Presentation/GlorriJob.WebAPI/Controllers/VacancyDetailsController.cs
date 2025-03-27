using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Application.Validations.VacancyDetail;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GlorriJob.WebAPI.Controllers
{
	[Route("api/v1/[controller]/[action]")]
	[ApiController]
	public class VacancyDetailsController : ControllerBase
	{
		private readonly IVacancyDetailService _vacancyDetailService;
        public VacancyDetailsController(IVacancyDetailService vacancyDetailService)
        {
            _vacancyDetailService = vacancyDetailService;
        }

		[HttpPost]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> Create(VacancyDetailCreateDto vacancyDetailCreateDto)
		{
			var result = await _vacancyDetailService.CreateAsync(vacancyDetailCreateDto);
			return StatusCode((int)result.StatusCode, result);
		}
		[HttpDelete]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _vacancyDetailService.DeleteAsync(id);
			return StatusCode((int)result.StatusCode, result);
		}
		[HttpGet]
		[Authorize(Policy = "UserPolicy")]
		public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			var result = await _vacancyDetailService.GetAllAsync(pageNumber, pageSize, isPaginated);
			return StatusCode((int)result.StatusCode, result);
		}
		[HttpGet("{id}")]
		[Authorize(Policy = "UserPolicy")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _vacancyDetailService.GetByIdAsync(id);
			return StatusCode((int)result.StatusCode, result);
		}
		[HttpPut]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> Update(Guid id, VacancyDetailUpdateDto vacancyDetailUpdateDto)
		{
			var result = await _vacancyDetailService.UpdateAsync(id, vacancyDetailUpdateDto);
			return StatusCode((int)result.StatusCode, result);
		}
	}
}
