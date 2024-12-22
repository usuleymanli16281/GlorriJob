using FluentValidation;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GlorriJob.WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class VacancyDetailsController : ControllerBase
    {
        private readonly IVacancyDetailService _vacancyDetailService;

        public VacancyDetailsController(IVacancyDetailService vacancyDetailService)
        {
            _vacancyDetailService = vacancyDetailService;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<VacancyDetailGetDto>>> GetAllAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
        {
            try
            {
                var result = await _vacancyDetailService.GetAllAsync(pageNumber, pageSize, isPaginated);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<Pagination<VacancyDetailGetDto>>> SearchByContentAsync(
            [FromQuery] string content,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
        {
            try
            {
                var result = await _vacancyDetailService.SearchByContentAsync(content, pageNumber, pageSize, isPaginated);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VacancyDetailGetDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var result = await _vacancyDetailService.GetByIdAsync(id);
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
        public async Task<ActionResult<VacancyDetailGetDto>> CreateAsync([FromBody] VacancyDetailCreateDto vacancyDetailCreateDto)
        {
            try
            {
                var result = await _vacancyDetailService.CreateAsync(vacancyDetailCreateDto);
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
        public async Task<ActionResult<VacancyDetailUpdateDto>> UpdateAsync(
            Guid id,
            [FromBody] VacancyDetailUpdateDto vacancyDetailUpdateDto)
        {
            try
            {
                var result = await _vacancyDetailService.UpdateAsync(id, vacancyDetailUpdateDto);
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
                await _vacancyDetailService.DeleteAsync(id);
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


}
