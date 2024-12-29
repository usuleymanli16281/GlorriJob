using FluentValidation;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Industry;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GlorriJob.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IndustriesController : ControllerBase
{
    private readonly IIndustryService _industryService;

    public IndustriesController(IIndustryService industryService)
    {
        _industryService = industryService;
    }

    [HttpGet]
    public async Task<ActionResult<Pagination<IndustryGetDto>>> GetAllAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isPaginated = true)
    {
        var result = await _industryService.GetAllAsync(pageNumber, pageSize, isPaginated);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<Pagination<IndustryGetDto>>> SearchByNameAsync(
        [FromQuery] string name,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isPaginated = true)
    {
        var result = await _industryService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IndustryGetDto>> GetByIdAsync(Guid id)
    {
        var result = await _industryService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<IndustryGetDto>> CreateAsync([FromBody] IndustryCreateDto industryCreateDto)
    {
        var result = await _industryService.CreateAsync(industryCreateDto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<IndustryUpdateDto>> UpdateAsync(
        Guid id,
        [FromBody] IndustryUpdateDto industryUpdateDto)
    {
        var result = await _industryService.UpdateAsync(id, industryUpdateDto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await _industryService.DeleteAsync(id);
        return NoContent();
    }

}


