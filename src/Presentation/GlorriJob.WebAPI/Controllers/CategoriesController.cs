using FluentValidation;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GlorriJob.WebAPI.Controllers;

[Route("api/v1/[controller]/[action]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [Authorize(Policy = "UserPolicy")]
    public async Task<ActionResult<Pagination<CategoryGetDto>>> GetAllAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isPaginated = true)
    {
        var result = await _categoryService.GetAllAsync(pageNumber, pageSize, isPaginated);
        return Ok(result);
    }

    [HttpGet("search")]
	[Authorize(Policy = "UserPolicy")]
	public async Task<ActionResult<Pagination<CategoryGetDto>>> SearchByNameAsync(
        [FromQuery] string name,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isPaginated = true)
    {
        var result = await _categoryService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
        return Ok(result);
    }

    [HttpGet("{id}")]
	[Authorize(Policy = "UserPolicy")]
	public async Task<ActionResult<CategoryGetDto>> GetByIdAsync(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<ActionResult<CategoryGetDto>> CreateAsync([FromBody] CategoryCreateDto categoryCreateDto)
    {
        var result = await _categoryService.CreateAsync(categoryCreateDto);
        return Ok(result);
    }

    [HttpPut("{id}")]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<ActionResult<CategoryUpdateDto>> UpdateAsync(
        Guid id,
        [FromBody] CategoryUpdateDto categoryUpdateDto)
    {
        var result = await _categoryService.UpdateAsync(id, categoryUpdateDto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
	[Authorize(Policy = "AdminPolicy")]
	public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await _categoryService.DeleteAsync(id);
        return NoContent();
    }
}

