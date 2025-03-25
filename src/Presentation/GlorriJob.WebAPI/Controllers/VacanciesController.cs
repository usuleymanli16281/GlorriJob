using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Vacancy;
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
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var response = await _vacancyService.GetByIdAsync(id);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetVacanciesAsync([FromQuery] VacancyFilterDto filterDto)
    {
        var response = await _vacancyService.GetVacanciesAsync(filterDto);
        return Ok(response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchVacanciesAsync([FromQuery] VacancyFilterDto filterDto)
    {
        var response = await _vacancyService.SearchVacanciesAsync(filterDto);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] VacancyCreateDto createVacancyDto)
    {
        var response = await _vacancyService.CreateAsync(createVacancyDto);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] VacancyUpdateDto vacancyUpdateDto)
    {
        var response = await _vacancyService.UpdateAsync(id, vacancyUpdateDto);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var response = await _vacancyService.DeleteAsync(id);
        return Ok(response);

    }
}

