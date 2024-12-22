using FluentValidation;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos;
using GlorriJob.Application.Dtos.Vacancy;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GlorriJob.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacanciesController : ControllerBase
    {
        private readonly IVacancyService _vacancyService;

        public VacanciesController(IVacancyService vacancyService)
        {
            _vacancyService = vacancyService;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<VacancyGetDto>>> GetAllAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
        {
            try
            {
                var result = await _vacancyService.GetAllAsync(pageNumber, pageSize, isPaginated);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<Pagination<VacancyGetDto>>> SearchByNameAsync(
            [FromQuery] string title,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
        {
            try
            {
                var result = await _vacancyService.SearchByNameAsync(title, pageNumber, pageSize, isPaginated);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VacancyGetDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var result = await _vacancyService.GetByIdAsync(id);
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
        public async Task<ActionResult<VacancyGetDto>> CreateAsync([FromBody] VacancyCreateDto vacancyCreateDto)
        {
            try
            {
                var result = await _vacancyService.CreateAsync(vacancyCreateDto);
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
        public async Task<ActionResult<VacancyUpdateDto>> UpdateAsync(
            Guid id,
            [FromBody] VacancyUpdateDto vacancyUpdateDto)
        {
            try
            {
                var result = await _vacancyService.UpdateAsync(id, vacancyUpdateDto);
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
                await _vacancyService.DeleteAsync(id);
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

        [HttpPost("filter")]
        public async Task<ActionResult<Pagination<VacancyGetDto>>> FilterVacanciesAsync(
            [FromBody] VacancyFilterDto filterDto,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
        {
            try
            {
                var result = await _vacancyService.FilterVacanciesAsync(filterDto, pageNumber, pageSize, isPaginated);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }


}
