using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.City;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GlorriJob.WebAPI.Controllers
{
  [Route("api/v1/[controller]")]
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
			var response = await _cityService.GetAllAsync(pageNumber, pageSize, isPaginated);
			return StatusCode((int)response.StatusCode, response);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var response = await _cityService.GetByIdAsync(id);
			return StatusCode((int)response.StatusCode, response); ;
		}
		[HttpGet("search")]
		public async Task<IActionResult> SearchByName(
		[FromQuery]string name,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isPaginated = true)
        {
			var response = await _cityService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
			return StatusCode((int)response.StatusCode, response);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CityCreateDto cityCreateDto)
		{
			var response =  await _cityService.CreateAsync(cityCreateDto);
			return StatusCode((int)response.StatusCode, response);
		}

		
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] CityUpdateDto cityUpdateDto)
		{
			var response = await _cityService.UpdateAsync(id, cityUpdateDto);
			return StatusCode((int)response.StatusCode, response);
		}

		
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var response = await _cityService.DeleteAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}
