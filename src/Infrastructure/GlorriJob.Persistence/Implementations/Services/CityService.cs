using AutoMapper;
using FluentValidation;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Validations.City;
using GlorriJob.Common.Shared;
using GlorriJob.Domain;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;


namespace GlorriJob.Persistence.Implementations.Services;

public class CityService : ICityService
{
    private ICityRepository _cityRepository { get; }
    private IMapper _mapper { get; }

    public CityService(ICityRepository cityRepository, IMapper mapper)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CityGetDto>> CreateAsync(CityCreateDto cityCreateDto)
    {
        var validator = new CityCreateValidator();
        var validationResult = await validator.ValidateAsync(cityCreateDto);

        if (!validationResult.IsValid)
        {
            return new ApiResponse<CityGetDto>(
                "Validation failed: " + string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                ResponseStatusCode.BadRequest.ToString(),
                null
            );
        }

        var existedCity = await _cityRepository.GetByFilter(
            expression: c => c.Name == cityCreateDto.Name && !c.IsDeleted,
            isTracking: false
        );

        if (existedCity is not null)
        {
            return new ApiResponse<CityGetDto>(
                $"A city with the name '{cityCreateDto.Name}' already exists.",
                ResponseStatusCode.Conflict.ToString(),
                null
            );
        }

        var createdCity = _mapper.Map<City>(cityCreateDto);
        await _cityRepository.AddAsync(createdCity);
        await _cityRepository.SaveChangesAsync();

        return new ApiResponse<CityGetDto>(
            "City created successfully.",
            ResponseStatusCode.Created.ToString(),
            _mapper.Map<CityGetDto>(createdCity)
        );
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        if (city is null || city.IsDeleted)
        {
            return new ApiResponse<bool>(
                "This city does not exist.",
                ResponseStatusCode.NotFound.ToString(),
                false
            );
        }

        city.IsDeleted = true;
        await _cityRepository.SaveChangesAsync();

        return new ApiResponse<bool>(
            "City deleted successfully.",
            ResponseStatusCode.Success.ToString(),
            true
        );
    }

    public async Task<ApiResponse<Pagination<CityGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return new ApiResponse<Pagination<CityGetDto>>(
                "Page number and page size should be greater than 0.",
                ResponseStatusCode.BadRequest.ToString(),
                null
            );
        }

        IQueryable<City> query = _cityRepository.GetAll(c => !c.IsDeleted);
        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<CityGetDto> cityGetDtos = await query
            .Select(c => new CityGetDto { Id = c.Id, Name = c.Name })
            .ToListAsync();

        var pagination = new Pagination<CityGetDto>
        {
            Items = cityGetDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize)
        };

        return new ApiResponse<Pagination<CityGetDto>>(
            "Cities fetched successfully.",
            ResponseStatusCode.Success.ToString(),
            pagination
        );
    }

    public async Task<ApiResponse<CityGetDto>> GetByIdAsync(Guid id)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        if (city is null || city.IsDeleted)
        {
            return new ApiResponse<CityGetDto>(
                "This city does not exist.",
                ResponseStatusCode.NotFound.ToString(),
                null
            );
        }

        var cityGetDto = _mapper.Map<CityGetDto>(city);
        return new ApiResponse<CityGetDto>(
            "City fetched successfully.",
            ResponseStatusCode.Success.ToString(),
            cityGetDto
        );
    }

    public async Task<ApiResponse<Pagination<CityGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return new ApiResponse<Pagination<CityGetDto>>(
                "Page number and page size should be greater than 0.",
                ResponseStatusCode.BadRequest.ToString(),
                null
            );
        }

        IQueryable<City> query = _cityRepository.GetAll(c => !c.IsDeleted && c.Name.ToLower().Contains(name.ToLower()));
        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<CityGetDto> cityGetDtos = await query
            .Select(c => new CityGetDto { Id = c.Id, Name = c.Name })
            .ToListAsync();

        var pagination = new Pagination<CityGetDto>
        {
            Items = cityGetDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize)
        };

        return new ApiResponse<Pagination<CityGetDto>>(
            "Cities search completed successfully.",
            ResponseStatusCode.Success.ToString(),
            pagination
        );
    }

    public async Task<ApiResponse<CityUpdateDto>> UpdateAsync(Guid id, CityUpdateDto cityUpdateDto)
    {
        if (id != cityUpdateDto.Id)
        {
            return new ApiResponse<CityUpdateDto>(
                "The ID provided does not match the ID in the URL parameter.",
                ResponseStatusCode.BadRequest.ToString(),
                null
            );
        }

        var validator = new CityUpdateValidator();
        var validationResult = await validator.ValidateAsync(cityUpdateDto);

        if (!validationResult.IsValid)
        {
            return new ApiResponse<CityUpdateDto>(
                "Validation failed: " + string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                ResponseStatusCode.BadRequest.ToString(),
                null
            );
        }

        var city = await _cityRepository.GetByIdAsync(id);
        if (city is null || city.IsDeleted)
        {
            return new ApiResponse<CityUpdateDto>(
                "The requested city does not exist or is deleted.",
                ResponseStatusCode.NotFound.ToString(),
                null
            );
        }

        city.Name = cityUpdateDto.Name;
        _cityRepository.Update(city);
        await _cityRepository.SaveChangesAsync();

        var updatedCityDto = _mapper.Map<CityUpdateDto>(city);

        return new ApiResponse<CityUpdateDto>(
            "City updated successfully.",
            ResponseStatusCode.Success.ToString(),
            updatedCityDto
        );
    }
}

