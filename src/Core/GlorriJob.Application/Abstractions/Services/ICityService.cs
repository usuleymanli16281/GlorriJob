using GlorriJob.Application.Dtos;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface ICityService
{
    Task<GetCityDto> GetByIdAsync(Guid id);
    Task<Pagination<GetCityDto>> GetAll(int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<Pagination<GetCityDto>> SearchByName(string name, int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<GetCityDto> CreateAsync(CreateCityDto createCityDto);
    Task<GetCityDto> UpdateAsync(CreateCityDto updateCityDto);
    Task DeleteAsync(Guid id);

}