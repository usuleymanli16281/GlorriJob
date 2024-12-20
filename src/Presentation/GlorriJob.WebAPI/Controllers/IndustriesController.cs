using FluentValidation;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Industry;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.AspNetCore.Mvc;

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
        try
        {
            var result = await _industryService.GetAllAsync(pageNumber, pageSize, isPaginated);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<Pagination<IndustryGetDto>>> SearchByNameAsync(
        [FromQuery] string name,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isPaginated = true)
    {
        try
        {
            var result = await _industryService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IndustryGetDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var result = await _industryService.GetByIdAsync(id);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<IndustryGetDto>> CreateAsync([FromBody] IndustryCreateDto industryCreateDto)
    {
        try
        {
            var result = await _industryService.CreateAsync(industryCreateDto);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
        catch (AlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<IndustryUpdateDto>> UpdateAsync(
        Guid id,
        [FromBody] IndustryUpdateDto industryUpdateDto)
    {
        try
        {
            var result = await _industryService.UpdateAsync(id, industryUpdateDto);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        try
        {
            await _industryService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

}


