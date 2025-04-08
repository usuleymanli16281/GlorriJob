using GlorriJob.Application.Dtos.Vacancy;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface IVacancyService
{
    Task<BaseResponse<VacancyGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<Pagination<VacancyGetDto>>> GetAllAsync(VacancyFilterDto filterDto, int pageNumber = 1, int pageSize= 10, bool isPaginated = true);
    Task<BaseResponse<object>> CreateAsync(VacancyCreateDto createVacancyDto);
    Task<BaseResponse<object>> UpdateAsync(Guid id, VacancyUpdateDto vacancyUpdateDto);
    Task<BaseResponse<object>> DeleteAsync(Guid id);
}





