using GlorriJob.Application.Dtos.Industry;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public  interface IIndustryService
{
    Task<BaseResponse<IndustryGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<Pagination<IndustryGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<Pagination<IndustryGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<IndustryGetDto>> CreateAsync(IndustryCreateDto createIndustryDto);
    Task<BaseResponse<IndustryGetDto>> UpdateAsync(Guid id, IndustryUpdateDto industryUpdateDto);
    Task<BaseResponse<object>> DeleteAsync(Guid id);


}
