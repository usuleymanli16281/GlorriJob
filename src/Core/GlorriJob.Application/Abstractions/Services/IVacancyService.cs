using GlorriJob.Application.Dtos.Category;
using GlorriJob.Application.Dtos.Vacancy;
using GlorriJob.Common.Shared;
using GlorriJob.Domain;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface IVacancyService
{
    Task<BaseResponse<VacancyGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<Pagination<VacancyGetDto>>> GetVacanciesAsync(VacancyFilterDto filterDto);
    Task<BaseResponse<Pagination<VacancyGetDto>>> SearchVacanciesAsync(VacancyFilterDto filterDto);
    Task<BaseResponse<VacancyGetDto>> CreateAsync(VacancyCreateDto createVacancyDto);
    Task<BaseResponse<VacancyGetDto>> UpdateAsync(Guid id, VacancyUpdateDto vacancyUpdateDto);
    Task<BaseResponse<object>> DeleteAsync(Guid id);
}





