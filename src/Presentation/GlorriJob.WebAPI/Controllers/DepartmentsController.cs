using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Department;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlorriJob.WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DepartmentsController : ControllerBase
	{
		private IDepartmentService _departmentService { get; }
		public DepartmentsController(IDepartmentService departmentService)
		{
			_departmentService = departmentService;
		}

		[HttpGet("all")]
		public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
		{
			var data = await _departmentService.GetAllAsync(pageNumber, pageSize, isPaginated);
			return Ok(data);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var data = await _departmentService.GetByIdAsync(id);
			return Ok(data);
		}
		[HttpGet]
		public async Task<IActionResult> SearchByName(
			[FromQuery] string name,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isPaginated = true)
		{
			var data = await _departmentService.SearchByNameAsync(name, pageNumber, pageSize, isPaginated);
			return Ok(data);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] DepartmentCreateDto departmentCreateDto)
		{
			var data = await _departmentService.CreateAsync(departmentCreateDto);
			return Ok(data);
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] DepartmentUpdateDto departmentUpdateDto)
		{
			var data = await _departmentService.UpdateAsync(id, departmentUpdateDto);
			return Ok(data);
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			await _departmentService.DeleteAsync(id);
			return Ok();
		}
	}
}
