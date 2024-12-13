using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GlorriJob.Persistence.Implementations.Services;

internal class CityService : ICityService
{
    private ICityRepository _cityRepository { get; }
    private IMapper _mapper { get; }
    public CityService(ICityRepository cityRepository, IMapper mapper)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
    }
    public async Task<CityGetDto> CreateAsync(CityCreateDto createCityDto)
    {
        var existingCity = await _cityRepository.GetFiltered(
            c => c.Name == createCityDto.Name && !c.IsDeleted,
            isTracking: false);

        if (existingCity is not null)
        {
            throw new CityAlreadyExistsException($"A city with the name '{createCityDto.Name}' already exists.");
        }

        var city = new City
        {
            Name = createCityDto.Name,
        };
        await _cityRepository.AddAsync(city);
        await _cityRepository.SaveChangesAsync();
        var getCityDto = _mapper.Map<CityGetDto>(city);
        return getCityDto;
    }

    public async Task DeleteAsync(Guid id)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        if (city is null || city.IsDeleted)
        {
            throw new CityNotFoundException("The city you are trying to delete does not exist or already has been deleted.");
        }
        city.IsDeleted = true;
        await _cityRepository.SaveChangesAsync();
    }

	public async Task<Pagination<CityGetDto>> GetAllAsync(int pageNumber = 1, int take = 10, bool isPaginated = false)
	{
		if (pageNumber < 1 || take < 1)
			throw new InvalidPageArgumentException("Page number and take must be greater than 0.");

		IQueryable<City> query = _cityRepository.GetAll(c => !c.IsDeleted);
        int totalItems = await query.CountAsync();
		if (totalItems == 0)
			throw new CityNotFoundException("No cities found.");
        if (isPaginated)
        {
            int skip = (pageNumber - 1) * take;
            query = _cityRepository.GetAll(expression: c => !c.IsDeleted, skip: skip, take: take);
        }
		List<City> cities = await query.ToListAsync();
		if (isPaginated && !cities.Any())
			throw new PageOutOfRangeException("No cities available for the requested page.");

		List<CityGetDto> cityDtos = _mapper.Map<List<CityGetDto>>(cities);

		return new Pagination<CityGetDto>
		{
			Items = cityDtos,
			TotalCount = totalItems,
			PageIndex = pageNumber,
			PageSize = isPaginated ? take : totalItems
		};
	}

	public async Task<CityGetDto> GetByIdAsync(Guid id)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        if (city is null || city.IsDeleted)
        {
            throw new CityNotFoundException("The city you are trying to get does not exist or has been deleted.");
        }

        var getCityDto = _mapper.Map<CityGetDto>(city);
        return getCityDto;
    }

    public async Task<Pagination<CityGetDto>> SearchByNameAsync(string name, int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || take < 1)
        {
            throw new InvalidPageArgumentException("Page number and take must be greater than 0.");
        }
        IQueryable<City> query = _cityRepository.GetAll(c => !c.IsDeleted && c.Name.ToLower().Contains(name.ToLower()));
        int totalItems = await query.CountAsync();
        if (totalItems == 0)
        {
            throw new CityNotFoundException($"No cities found matching the name '{name}'.");
        }
        if (isPaginated)
        {
			int skip = (pageNumber - 1) * take;
			query = _cityRepository.GetAll(expression: c => !c.IsDeleted && c.Name.ToLower().Contains(name.ToLower()), skip: skip, take: take);
		}
        List<City> cities = await query.ToListAsync();
		if (isPaginated && !cities.Any())
			throw new PageOutOfRangeException("No cities available for the requested page.");

		List<CityGetDto> cityDtos = _mapper.Map<List<CityGetDto>>(cities);
        Pagination<CityGetDto> pagination = new Pagination<CityGetDto>
        {
            Items = cityDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : totalItems
        };
        return pagination;
    }

    public async Task<CityUpdateDto> UpdateAsync(Guid id, CityUpdateDto cityUpdateDto)
    {
        if(id != cityUpdateDto.Id)
        {
            throw new BadRequestException("Id doesn't match with root");
        }
        var city = await _cityRepository.GetByIdAsync(id);
        if (city is null || city.IsDeleted)
        {
            throw new CityNotFoundException("The city you are trying to update does not exist or has been deleted.");
        }
        var modifiedCity = _mapper.Map<City>(cityUpdateDto);
        _cityRepository.Update(modifiedCity);
        await _cityRepository.SaveChangesAsync();
        return cityUpdateDto;
    }
}
