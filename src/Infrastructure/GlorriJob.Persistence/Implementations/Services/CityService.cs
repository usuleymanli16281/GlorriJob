﻿using AutoMapper;
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
        if (string.IsNullOrEmpty(createCityDto.Name) || createCityDto.Name.Length < 3)
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

    public async Task<Pagination<GetCityDto>> GetAll(int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || take < 1)
        {
            throw new InvalidPageArgumentException("Page number and take must be greater than 0.");
        }
        int skip = (pageNumber - 1) * take;
        IQueryable<City> query = _cityRepository.GetAll()
            .Where(c => !c.IsDeleted);
        int totalItems = await query.CountAsync();
        if (totalItems is 0)
        {
            throw new CityNotFoundException("No cities found");
        }
        List<City> cities = await query.ToListAsync();

        if (cities.Count is 0)
        {
            throw new PageOutOfRangeException("No cities available for the requested page.");
        }
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

    public async Task<Pagination<GetCityDto>> SearchByName(string name, int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new CityNameEmptyException("Search term cannot be null or empty.");
        }
        if (pageNumber < 1 || take < 1)
        {
            throw new InvalidPageArgumentException("Page number and take must be greater than 0.");
        }
        int skip = (pageNumber - 1) * take;
        IQueryable<City> query = _cityRepository.GetAll()
            .Where(c => !c.IsDeleted && c.Name
            .Contains(name));
        int totalItems = await query.CountAsync();
        if (totalItems is 0)
        {
            throw new CityNotFoundException($"No cities found matching the name '{name}'.");
        }
        if (isPaginated)
        {
            query = query.Skip(skip).Take(take);
        }
        List<City> cities = await query.ToListAsync();
        if (cities.Count is 0)
        {
            throw new PageOutOfRangeException("No cities available for the requested page.");
        }
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
