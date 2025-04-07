
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;


namespace GlorriJob.Application.Abstractions.Services
{
	public interface IVacancyDetailService
	{
		Task<BaseResponse<VacancyDetailGetDto>> GetByIdAsync(Guid id);
		Task<BaseResponse<Pagination<VacancyDetailGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
		Task<BaseResponse<object>> CreateAsync(VacancyDetailCreateDto vacancyDetailCreateDto);
		Task<BaseResponse<object>> UpdateAsync(Guid id, VacancyDetailUpdateDto vacancyDetailUpdateDto);
		Task<BaseResponse<object>> DeleteAsync(Guid id);
	}
}
