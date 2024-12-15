using GlorriJob.Application.Dtos;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public  interface IIndustryService
{
    Task<IndustryGetDto> GetByIdAsync(Guid id);
    Task<Pagination<IndustryGetDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<Pagination<IndustryGetDto>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<IndustryGetDto> CreateAsync(IndustryCreateDto createIndustryDto);
    Task<IndustryUpdateDto> UpdateAsync(Guid id, IndustryUpdateDto industryUpdateDto);
    Task DeleteAsync(Guid id);


}
