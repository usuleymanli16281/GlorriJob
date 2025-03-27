using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Vacancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GlorriJob.WebAPI.Controllers;

[Route("api/v1/[controller]/[action]")]
[ApiController]
public class VacancyController : ControllerBase
{
    private readonly IVacancyService _vacancyService;

    public VacancyController(IVacancyService vacancyService)
    {
        _vacancyService = vacancyService;
    }

   
    [HttpGet("{id}")]
	[Authorize(Policy = "UserPolicy")]
	public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _vacancyService.GetByIdAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet]
	[Authorize(Policy = "UserPolicy")]
	public async Task<IActionResult> GetVacanciesAsync([FromQuery] VacancyFilterDto filterDto)
    {
        var result = await _vacancyService.GetVacanciesAsync(filterDto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("search")]
	[Authorize(Policy = "UserPolicy")]
	public async Task<IActionResult> SearchVacanciesAsync([FromQuery] VacancyFilterDto filterDto)
    {
        var result = await _vacancyService.SearchVacanciesAsync(filterDto);
		return StatusCode((int)result.StatusCode, result);
	}

    [HttpPost]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<IActionResult> CreateAsync([FromBody] VacancyCreateDto createVacancyDto)
    {
        var result = await _vacancyService.CreateAsync(createVacancyDto);
		return StatusCode((int)result.StatusCode, result);
	}

    [HttpPut("{id}")]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] VacancyUpdateDto vacancyUpdateDto)
    {
        var result = await _vacancyService.UpdateAsync(id, vacancyUpdateDto);
		return StatusCode((int)result.StatusCode, result);
	}

    [HttpDelete("{id}")]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _vacancyService.DeleteAsync(id);
		return StatusCode((int)result.StatusCode, result);

	}
}

