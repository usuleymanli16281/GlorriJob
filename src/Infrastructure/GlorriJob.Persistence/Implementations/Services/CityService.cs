using AutoMapper;
using FluentValidation;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Validations.City;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;


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
    public async Task<CityGetDto> CreateAsync(CityCreateDto cityCreateDto)
    {
        var validator = new CityCreateValidator();
        var validationResult = await validator.ValidateAsync(cityCreateDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        var existedCity = await _cityRepository.GetByFilter(expression:
            c => c.Name == cityCreateDto.Name && !c.IsDeleted,
            isTracking: false);

        if (existedCity is not null)
        {
            throw new AlreadyExistsException($"A city with the name '{cityCreateDto.Name}' already exists.");
        }
        var createdCity = _mapper.Map<City>(cityCreateDto);
        await _cityRepository.AddAsync(createdCity);
        await _cityRepository.SaveChangesAsync();
        return _mapper.Map<CityGetDto>(createdCity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        if (city is null || city.IsDeleted)
        {
            throw new NotFoundException("This city does not exist.");
        }
        city.IsDeleted = true;
        await _cityRepository.SaveChangesAsync();
    }

    public async Task<Pagination<CityGetDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new BadRequestException("Page number and page size should be greater than 0.");

        IQueryable<City> query = _cityRepository.GetAll(c => !c.IsDeleted);
        int totalItems = await query.CountAsync();
        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }
        List<CityGetDto> cityGetDtos = await query.Select(c => new CityGetDto { Id = c.Id, Name = c.Name }).ToListAsync();
        return new Pagination<CityGetDto>
        {
            Items = cityGetDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };
    }

    public async Task<CityGetDto> GetByIdAsync(Guid id)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        if (city is null || city.IsDeleted)
        {
            throw new NotFoundException("This city does not exist.");
        }

        var cityGetDto = _mapper.Map<CityGetDto>(city);
        return cityGetDto;
    }

    public async Task<Pagination<CityGetDto>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new BadRequestException("Page number and page size should be greater than 0.");

        IQueryable<City> query = _cityRepository.GetAll(c => !c.IsDeleted && c.Name.ToLower().Contains(name.ToLower()));
        int totalItem = await query.CountAsync();
        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }
        List<CityGetDto> cityGetDtos = await query.Select(c => new CityGetDto { Id = c.Id, Name = c.Name }).ToListAsync();
        return new Pagination<CityGetDto>
        {
            Items = cityGetDtos,
            TotalCount = totalItem,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItem,
            TotalPage = (int)Math.Ceiling((double)totalItem / pageSize),
        };
    }

    public async Task<CityUpdateDto> UpdateAsync(Guid id, CityUpdateDto cityUpdateDto)
    {
        if (id != cityUpdateDto.Id)
        {
            throw new BadRequestException("Id doesn't match with the root");
        }
        var validator = new CityUpdateValidator();
        var validationResult = await validator.ValidateAsync(cityUpdateDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        var city = await _cityRepository.GetByIdAsync(id);
        if (city is null || city.IsDeleted)
        {
            throw new NotFoundException("This city does not exist.");
        }
        _mapper.Map(cityUpdateDto, city);
        _cityRepository.Update(city);
        await _cityRepository.SaveChangesAsync();
        return cityUpdateDto;
    }

}
