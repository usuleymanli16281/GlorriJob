using GlorriJob.Application.Dtos.Vacancy;
using GlorriJob.Domain.Shared;
using GlorriJob.Domain;
using GlorriJob.Application.Dtos;

namespace GlorriJob.Application.Abstractions.Services;

public interface IVacancyService
{
    Task<VacancyGetDto> GetByIdAsync(Guid id);
    Task<Pagination<VacancyGetDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<Pagination<VacancyGetDto>> SearchByNameAsync(string title, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<VacancyGetDto> CreateAsync(VacancyCreateDto createVacancyDto);
    Task<VacancyUpdateDto> UpdateAsync(Guid id, VacancyUpdateDto vacancyUpdateDto);
    Task DeleteAsync(Guid id);
    Task<Pagination<VacancyGetDto>> FilterVacanciesAsync(VacancyFilterDto filterDto, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);

}

