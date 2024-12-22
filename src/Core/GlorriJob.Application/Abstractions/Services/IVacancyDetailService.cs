using GlorriJob.Application.Dtos;
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface IVacancyDetailService
{
    Task<VacancyDetailGetDto> GetByIdAsync(Guid id);
    Task<Pagination<VacancyDetailGetDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<Pagination<VacancyDetailGetDto>> SearchByContentAsync(string content, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<VacancyDetailGetDto> CreateAsync(VacancyDetailCreateDto createVacancyDetailDto);
    Task<VacancyDetailUpdateDto> UpdateAsync(Guid id, VacancyDetailUpdateDto vacancyDetailUpdateDto);
    Task DeleteAsync(Guid id);
}

