using AutoMapper;
using FluentValidation;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Application.Validations.City;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;


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
	public async Task<BaseResponse<object>> CreateAsync(CityCreateDto cityCreateDto)
	{
		var validator = new CityCreateValidator();
		var validationResult = await validator.ValidateAsync(cityCreateDto);

		if (!validationResult.IsValid)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
			};
		}
		var existedCity = await _cityRepository.GetByFilter(expression:
			c => c.Name.ToLower() == cityCreateDto.Name.ToLower() && !c.IsDeleted,
			isTracking: false);

		if (existedCity is not null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "The city already exists."
			};
		}

		var createdCity = _mapper.Map<City>(cityCreateDto);
		await _cityRepository.AddAsync(createdCity);
		await _cityRepository.SaveChangesAsync();
		return new BaseResponse<object>
		{
			StatusCode = HttpStatusCode.Created,
			Message = "The city is successfully created"
		};
	}

	public async Task<BaseResponse<object>> DeleteAsync(Guid id)
	{
		var city = await _cityRepository.GetByIdAsync(id);
		if (city is null || city.IsDeleted)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "The city does not exist."
			};
		}

		city.IsDeleted = true;
		await _cityRepository.SaveChangesAsync();
		return new BaseResponse<object>
		{
			StatusCode = HttpStatusCode.OK,
			Message = "The city is successfully deleted"
		};
	}

	public async Task<BaseResponse<Pagination<CityGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
	{
		if (pageNumber < 1 || pageSize < 1)
		{
			return new BaseResponse<Pagination<CityGetDto>>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "Page number and page size should be greater than 0."
			};
		}
		IQueryable<City> query = _cityRepository.GetAll(c => !c.IsDeleted);
		int totalItems = await query.CountAsync();
		if (totalItems == 0)
		{
			return new BaseResponse<Pagination<CityGetDto>>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "The city does not exist"
			};
		}
		if (isPaginated)
		{
			int skip = (pageNumber - 1) * pageSize;
			query = query.Skip(skip).Take(pageSize);
		}
		List<CityGetDto> cityGetDtos = await query.Select(c => new CityGetDto { Id = c.Id, Name = c.Name }).ToListAsync();
		return new BaseResponse<Pagination<CityGetDto>>
		{
			StatusCode = HttpStatusCode.OK,
			Message = "Cities are successfully retrieved",
			Data = new Pagination<CityGetDto>
			{
				Items = cityGetDtos,
				TotalCount = totalItems,
				PageIndex = pageNumber,
				PageSize = isPaginated ? pageSize : totalItems,
				TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
			}
		};
	}

	public async Task<BaseResponse<CityGetDto>> GetByIdAsync(Guid id)
	{
		var city = await _cityRepository.GetByIdAsync(id);
		if (city is null || city.IsDeleted)
		{
			return new BaseResponse<CityGetDto>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "The city does not exist."
			};
		}
		return new BaseResponse<CityGetDto>
		{
			StatusCode = HttpStatusCode.OK,
			Message = "The city is successfully fetched.",
			Data = _mapper.Map<CityGetDto>(city)
		};

	}

	public async Task<BaseResponse<Pagination<CityGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
	{
		if (pageNumber < 1 || pageSize < 1)
		{
			return new BaseResponse<Pagination<CityGetDto>>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "Page number and page size should be greater than 0.",
				Data = null
			};
		}

		IQueryable<City> query = _cityRepository.GetAll(c => !c.IsDeleted && c.Name.ToLower().Contains(name.ToLower()));
		int totalItems = await query.CountAsync();
		if (totalItems == 0)
		{
			return new BaseResponse<Pagination<CityGetDto>>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "The city does not exist"
			};
		}
		if (isPaginated)
		{
			int skip = (pageNumber - 1) * pageSize;
			query = query.Skip(skip).Take(pageSize);
		}
		List<CityGetDto> cityGetDtos = await query.Select(c => new CityGetDto { Id = c.Id, Name = c.Name }).ToListAsync();
		return new BaseResponse<Pagination<CityGetDto>>
		{
			StatusCode = HttpStatusCode.OK,
			Message = "Cities are successfully retrieved",
			Data = new Pagination<CityGetDto>
			{
				Items = cityGetDtos,
				TotalCount = totalItems,
				PageIndex = pageNumber,
				PageSize = isPaginated ? pageSize : totalItems,
				TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
			}
		};
	}


	public async Task<BaseResponse<object>> UpdateAsync(Guid id, CityUpdateDto cityUpdateDto)
	{
		if (id != cityUpdateDto.Id)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "Id does not match with the root."
			};
		}
		var validator = new CityUpdateValidator();
		var validationResult = await validator.ValidateAsync(cityUpdateDto);

		if (!validationResult.IsValid)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = string.Join(";", validationResult.Errors.Select(e => e.ErrorMessage))
			};
		}
		var city = await _cityRepository.GetByIdAsync(id);
		if (city is null || city.IsDeleted)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "The city does not exist."
			};
		}
		var existingCity = await _cityRepository.GetByFilter(
		expression: c =>  city.Name.ToLower() != cityUpdateDto.Name.ToLower() && c.Name.ToLower() == cityUpdateDto.Name.ToLower() && c.Id != id && !c.IsDeleted,
		isTracking: false);
		if (existingCity is not null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = $"A city with the name '{cityUpdateDto.Name}' already exists."
			};
		}

		city.Name = cityUpdateDto.Name;
		_cityRepository.Update(city);
		await _cityRepository.SaveChangesAsync();
		return new BaseResponse<object>
		{
			StatusCode = HttpStatusCode.NoContent,
			Message = "The city is successfully updated."
		};

	}
}

