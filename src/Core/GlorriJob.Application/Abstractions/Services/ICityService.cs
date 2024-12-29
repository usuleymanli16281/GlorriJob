using GlorriJob.Application.Dtos;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface ICityService
{
    Task<ApiResponse<CityGetDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<Pagination<CityGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<ApiResponse<Pagination<CityGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<ApiResponse<CityGetDto>> CreateAsync(CityCreateDto createCityDto);
    Task<ApiResponse<CityUpdateDto>> UpdateAsync(Guid id, CityUpdateDto cityUpdateDto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);

}