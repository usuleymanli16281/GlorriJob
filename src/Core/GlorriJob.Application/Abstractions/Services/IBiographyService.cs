using GlorriJob.Application.Dtos.Biography;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;


namespace GlorriJob.Application.Abstractions.Services
{
	public interface IBiographyService
	{
		Task<BaseResponse<BiographyGetDto>> GetByIdAsync(Guid id);
		Task<BaseResponse<Pagination<BiographyGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
		Task<BaseResponse<object>> CreateAsync(BiographyCreateDto biographyCreateDto);
		Task<BaseResponse<object>> UpdateAsync(Guid id, BiographyUpdateDto biographyUpdateDto);
		Task<BaseResponse<object>> DeleteAsync(Guid id);
	}
}
