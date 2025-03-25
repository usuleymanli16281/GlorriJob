using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Application.Validations.VacancyDetail;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
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

		public async Task<IActionResult> Create(VacancyDetailCreateDto vacancyDetailCreateDto)
		{
			var response = await _vacancyDetailService.CreateAsync(vacancyDetailCreateDto);
			return StatusCode((int)response.StatusCode, response);
		}

		public async Task<IActionResult> Delete(Guid id)
		{
			var response = await _vacancyDetailService.DeleteAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}

		public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			var response = await _vacancyDetailService.GetAllAsync(pageNumber, pageSize, isPaginated);
			return StatusCode((int)response.StatusCode, response);
		}

		public async Task<IActionResult> GetById(Guid id)
		{
			var response = await _vacancyDetailService.GetByIdAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}

		public async Task<IActionResult> Update(Guid id, VacancyDetailUpdateDto vacancyDetailUpdateDto)
		{
			var response = await _vacancyDetailService.UpdateAsync(id, vacancyDetailUpdateDto);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}
