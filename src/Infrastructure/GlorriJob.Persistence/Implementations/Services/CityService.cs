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
    public async Task<GetCityDto> CreateAsync(CreateCityDto createCityDto)
    {
        if (string.IsNullOrWhiteSpace(createCityDto.Name) || createCityDto.Name.Length < 3)
        {
            throw new CityNameEmptyException("City name cannot be empty or too short. It must be at least 3 characters long.");
        }

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
        var getCityDto = _mapper.Map<GetCityDto>(city);
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

	public async Task<Pagination<GetCityDto>> GetAllAsync(int pageNumber = 1, int take = 10, bool isPaginated = false)
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

		List<GetCityDto> cityDtos = _mapper.Map<List<GetCityDto>>(cities);

		return new Pagination<GetCityDto>
		{
			Items = cityDtos,
			TotalCount = totalItems,
			PageIndex = pageNumber,
			PageSize = isPaginated ? take : totalItems
		};
	}

	public async Task<GetCityDto> GetByIdAsync(Guid id)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        if (city is null || city.IsDeleted)
        {
            throw new CityNotFoundException("The city you are trying to get does not exist or has been deleted.");
        }

        var getCityDto = _mapper.Map<GetCityDto>(city);
        return getCityDto;
    }

    public async Task<Pagination<GetCityDto>> SearchByNameAsync(string name, int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new CityNameEmptyException("Search term cannot be null or empty.");
        }
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

		List<GetCityDto> cityDtos = _mapper.Map<List<GetCityDto>>(cities);
        Pagination<GetCityDto> pagination = new Pagination<GetCityDto>
        {
            Items = cityDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : totalItems
        };
        return pagination;
    }

    public async Task<GetCityDto> UpdateAsync(Guid id, CreateCityDto updateCityDto)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        if (city is null || city.IsDeleted)
        {
            throw new CityNotFoundException("The city you are trying to update does not exist or has been deleted.");
        }
        city.Name = updateCityDto.Name;
        await _cityRepository.SaveChangesAsync();
        var getCityDto = _mapper.Map<GetCityDto>(city);
        return getCityDto;
    }
}
