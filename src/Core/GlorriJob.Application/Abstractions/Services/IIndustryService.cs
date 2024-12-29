using GlorriJob.Application.Dtos.Industry;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public  interface IIndustryService
{
    Task<ApiResponse<IndustryGetDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<Pagination<IndustryGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<ApiResponse<Pagination<IndustryGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<ApiResponse<IndustryGetDto>> CreateAsync(IndustryCreateDto createIndustryDto);
    Task<ApiResponse<IndustryUpdateDto>> UpdateAsync(Guid id, IndustryUpdateDto industryUpdateDto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);


}
