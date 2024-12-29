using GlorriJob.Application.Dtos;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface ICityService
{
    Task<BaseResponse<CityGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<Pagination<CityGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<Pagination<CityGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<BaseResponse<CityCreateDto>> CreateAsync(CityCreateDto createCityDto);
    Task<BaseResponse<CityUpdateDto>> UpdateAsync(Guid id, CityUpdateDto cityUpdateDto);
    Task<BaseResponse<object>> DeleteAsync(Guid id);
}