﻿using FluentValidation;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Industry;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlorriJob.WebAPI.Controllers;

[Route("api/v1/[controller]/[action]")]
[ApiController]
public class IndustriesController : ControllerBase
{
    private readonly IIndustryService _industryService;

    public IndustriesController(IIndustryService industryService)
    {
        _industryService = industryService;
    }

    [HttpGet]
	[Authorize(Policy = "UserPolicy")]
	public async Task<ActionResult<Pagination<IndustryGetDto>>> GetAllAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isPaginated = true)
    {
        var result = await _industryService.GetAllAsync(pageNumber, pageSize, isPaginated);
		return StatusCode((int)result.StatusCode, result);
	}

    [HttpGet("search")]
	[Authorize(Policy = "UserPolicy")]
	public async Task<ActionResult<Pagination<IndustryGetDto>>> SearchByNameAsync(
        [FromQuery] string name,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isPaginated = true)
    {
        var result = await _industryService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
		return StatusCode((int)result.StatusCode, result);
	}

    [HttpGet("{id}")]
	[Authorize(Policy = "UserPolicy")]
	public async Task<ActionResult<IndustryGetDto>> GetByIdAsync(Guid id)
    {
        var result = await _industryService.GetByIdAsync(id);
		return StatusCode((int)result.StatusCode, result);
	}

    [HttpPost]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<ActionResult<IndustryGetDto>> CreateAsync([FromBody] IndustryCreateDto industryCreateDto)
    {
        var result = await _industryService.CreateAsync(industryCreateDto);
		return StatusCode((int)result.StatusCode, result);
	}

    [HttpPut("{id}")]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<ActionResult<IndustryUpdateDto>> UpdateAsync(
        Guid id,
        [FromBody] IndustryUpdateDto industryUpdateDto)
    {
        var result = await _industryService.UpdateAsync(id, industryUpdateDto);
		return StatusCode((int)result.StatusCode, result);
	}

    [HttpDelete("{id}")]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<ActionResult> DeleteAsync(Guid id)
    {
        var result = await _industryService.DeleteAsync(id);
		return StatusCode((int)result.StatusCode, result);
	}

}


