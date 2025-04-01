using GlorriJob.Application.Dtos.Branch;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;


namespace GlorriJob.Application.Abstractions.Services
{
	public interface IBranchService
	{
		Task<BaseResponse<BranchGetDto>> GetByIdAsync(Guid id);
		Task<BaseResponse<Pagination<BranchGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
		Task<BaseResponse<object>> CreateAsync(BranchCreateDto branchCreateDto);
		Task<BaseResponse<object>> UpdateAsync(Guid id, BranchUpdateDto branchUpdateDto);
		Task<BaseResponse<object>> DeleteAsync(Guid id);
	}
}
