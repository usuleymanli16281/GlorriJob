using GlorriJob.Application.Dtos;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface ICityService
{
    Task<CityGetDto> GetByIdAsync(Guid id);
    Task<Pagination<CityGetDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<Pagination<CityGetDto>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
    Task<CityGetDto> CreateAsync(CityCreateDto createCityDto);
    Task<CityUpdateDto> UpdateAsync(Guid id, CityUpdateDto cityUpdateDto);
    Task DeleteAsync(Guid id);

}