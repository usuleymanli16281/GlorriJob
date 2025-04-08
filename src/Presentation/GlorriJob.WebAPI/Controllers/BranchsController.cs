using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Branch;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlorriJob.WebAPI.Controllers
{
	[Route("api/v1/[controller]/[action]")]
	[ApiController]
	public class BranchsController : ControllerBase
	{
		private IBranchService _branchService {  get; }
        public BranchsController(IBranchService branchService)
        {
            _branchService = branchService;
        }
        [HttpGet]
		[Authorize(Policy = "UserPolicy")]
		public async Task<ActionResult<Pagination<CategoryGetDto>>> GetAllAsync(
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10,
		[FromQuery] bool isPaginated = true)
		{
			var result = await _branchService.GetAllAsync(pageNumber, pageSize, isPaginated);
			return StatusCode((int)result.StatusCode, result);
		}

		[HttpGet("{id}")]
		[Authorize(Policy = "UserPolicy")]
		public async Task<ActionResult<CategoryGetDto>> GetByIdAsync(Guid id)
		{
			var result = await _branchService.GetByIdAsync(id);
			return StatusCode((int)result.StatusCode, result);
		}

		[HttpPost]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<ActionResult<CategoryGetDto>> CreateAsync([FromBody] BranchCreateDto branchCreateDto)
		{
			var result = await _branchService.CreateAsync(branchCreateDto);
			return StatusCode((int)result.StatusCode, result);
		}

		[HttpPut("{id}")]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<ActionResult<CategoryUpdateDto>> UpdateAsync(
			Guid id,
			[FromBody] BranchUpdateDto branchUpdateDto)
		{
			var result = await _branchService.UpdateAsync(id, branchUpdateDto);
			return StatusCode((int)result.StatusCode, result);
		}

		[HttpDelete("{id}")]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<ActionResult> DeleteAsync(Guid id)
		{
			var result = await _branchService.DeleteAsync(id);
			return StatusCode((int)result.StatusCode, result);
		}
	}
}
