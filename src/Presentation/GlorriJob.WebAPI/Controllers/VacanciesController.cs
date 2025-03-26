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
        var response = await _vacancyService.GetByIdAsync(id);
        return Ok(response);
    }

    [HttpGet]
	[Authorize(Policy = "UserPolicy")]
	public async Task<IActionResult> GetVacanciesAsync([FromQuery] VacancyFilterDto filterDto)
    {
        var response = await _vacancyService.GetVacanciesAsync(filterDto);
        return Ok(response);
    }

    [HttpGet("search")]
	[Authorize(Policy = "UserPolicy")]
	public async Task<IActionResult> SearchVacanciesAsync([FromQuery] VacancyFilterDto filterDto)
    {
        var response = await _vacancyService.SearchVacanciesAsync(filterDto);
        return Ok(response);
    }

    [HttpPost]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<IActionResult> CreateAsync([FromBody] VacancyCreateDto createVacancyDto)
    {
        var response = await _vacancyService.CreateAsync(createVacancyDto);
        return Ok(response);
    }

    [HttpPut("{id}")]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] VacancyUpdateDto vacancyUpdateDto)
    {
        var response = await _vacancyService.UpdateAsync(id, vacancyUpdateDto);
        return Ok(response);
    }

    [HttpDelete("{id}")]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var response = await _vacancyService.DeleteAsync(id);
        return Ok(response);

    }
}

