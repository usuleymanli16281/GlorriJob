﻿using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Vacancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace GlorriJob.WebAPI.Controllers;

[Route("api/v1/[controller]/[action]")]
[ApiController]
public class VacanciesController : ControllerBase
{
    private readonly IVacancyService _vacancyService;

    public VacanciesController(IVacancyService vacancyService)
    {
        _vacancyService = vacancyService;
    }

   
    [HttpGet("{id}")]
	[Authorize(Policy = "UserPolicy")]
	public async Task<IActionResult> GetById(Guid id)
    {
		var result = await _vacancyService.GetByIdAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet]
	[Authorize(Policy = "UserPolicy")]
	public async Task<IActionResult> GetAll([FromQuery] VacancyFilterDto filterDto, int pageNumber = 1, int pageSize = 10, bool isPaginated = true)
    {
        var result = await _vacancyService.GetAllAsync(filterDto, pageNumber, pageSize, isPaginated);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<IActionResult> Create([FromBody] VacancyCreateDto createVacancyDto)
    {
        var result = await _vacancyService.CreateAsync(createVacancyDto);
		return StatusCode((int)result.StatusCode, result);
	}

    [HttpPut("{id}")]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<IActionResult> Update(Guid id, [FromBody] VacancyUpdateDto vacancyUpdateDto)
    {
        var result = await _vacancyService.UpdateAsync(id, vacancyUpdateDto);
		return StatusCode((int)result.StatusCode, result);
	}

    [HttpDelete("{id}")]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _vacancyService.DeleteAsync(id);
		return StatusCode((int)result.StatusCode, result);

	}
}

