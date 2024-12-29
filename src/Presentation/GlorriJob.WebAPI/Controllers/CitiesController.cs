using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.City;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GlorriJob.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private ICityService _cityService { get; }
        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
        {
            var data = await _cityService.GetAllAsync(pageNumber, pageSize, isPaginated);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _cityService.GetByIdAsync(id);
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> SearchByName(
            [FromQuery] string name,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
        {
            var data = await _cityService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CityCreateDto cityCreateDto)
        {
            var data = await _cityService.CreateAsync(cityCreateDto);
            return Ok(data);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CityUpdateDto cityUpdateDto)
        {
            var data = await _cityService.UpdateAsync(id, cityUpdateDto);
            return Ok(data);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _cityService.DeleteAsync(id);
            return Ok();
        }
    }
}
