
using AutoMapper;
using FluentValidation;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Application.Dtos.Industry;
using GlorriJob.Application.Validations.Industry;
using GlorriJob.Common.Shared;
using GlorriJob.Domain;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;


namespace GlorriJob.Persistence.Implementations.Services;

public class IndustryService : IIndustryService
{
    private readonly IIndustryRepository _industryRepository;
    private readonly IMapper _mapper;

    public IndustryService(IIndustryRepository industryRepository, IMapper mapper)
    {
        _industryRepository = industryRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<IndustryGetDto>> CreateAsync(IndustryCreateDto industryCreateDto)
    {
        var validator = new IndustryCreateValidator();
        var validationResult = await validator.ValidateAsync(industryCreateDto);

        if (!validationResult.IsValid)
        {
			return new BaseResponse<IndustryGetDto>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
				Data = null
			};
        }

        var existedIndustry = await _industryRepository.GetByFilter(
            expression: i => i.Name == industryCreateDto.Name && !i.IsDeleted,
            isTracking: false);

        if (existedIndustry is not null)
        {
			return new BaseResponse<IndustryGetDto>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "The industry already exists.",
				Data = null
			};
		}

        var createdIndustry = _mapper.Map<Industry>(industryCreateDto);
        await _industryRepository.AddAsync(createdIndustry);
        await _industryRepository.SaveChangesAsync();

        var industryGetDto = _mapper.Map<IndustryGetDto>(createdIndustry);

		return new BaseResponse<IndustryGetDto>
		{
			StatusCode = HttpStatusCode.Created,
			Message = "The industry is successfully created.",
			Data = industryGetDto
		};

	}

    public async Task<BaseResponse<object>> DeleteAsync(Guid id)
    {
        var industry = await _industryRepository.GetByIdAsync(id);
        if (industry is null || industry.IsDeleted)
        {
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "The industry does not exist"
			};
		}
        industry.IsDeleted = true;
        await _industryRepository.SaveChangesAsync();

		return new BaseResponse<object>
		{
			StatusCode = HttpStatusCode.OK,
			Message = "The industry is successfully deleted"
		};
	}


    public async Task<BaseResponse<Pagination<IndustryGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
			return new BaseResponse<Pagination<IndustryGetDto>>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "Page number and page size should be greater than 0."
			};
		}
        IQueryable<Industry> query = _industryRepository.GetAll(i => !i.IsDeleted);

        int totalItems = await query.CountAsync();
        if(totalItems == 0)
        {
            return new BaseResponse<Pagination<IndustryGetDto>>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The industry does not exist"
            };
        } 
        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<IndustryGetDto> industryGetDtos = await query.Select(i => new IndustryGetDto { Id = i.Id, Name = i.Name }).ToListAsync();
        var pagination = new Pagination<IndustryGetDto>
        {
            Items = industryGetDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };

		return new BaseResponse<Pagination<IndustryGetDto>>
		{
			StatusCode = HttpStatusCode.OK,
			Message = "Industries are successfully fetched.",
			Data = pagination
		};
	}


    public async Task<BaseResponse<IndustryGetDto>> GetByIdAsync(Guid id)
    {
        var industry = await _industryRepository.GetByIdAsync(id);
        if (industry is null || industry.IsDeleted)
        {
			return new BaseResponse<IndustryGetDto>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "The industry does not exist"
			};
		}
        var industryGetDto = _mapper.Map<IndustryGetDto>(industry);

		return new BaseResponse<IndustryGetDto>
		{
			StatusCode = HttpStatusCode.OK,
			Message = "The industry is successfully fetched",
			Data = industryGetDto
		};
	}

    public async Task<BaseResponse<Pagination<IndustryGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
			return new BaseResponse<Pagination<IndustryGetDto>>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "Page number and page size should be greater than 0."
			};
		}

        IQueryable<Industry> query = _industryRepository.GetAll(i => !i.IsDeleted && i.Name.ToLower().Contains(name.ToLower()));

        int totalItem = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<IndustryGetDto> industryGetDtos = await query.Select(i => new IndustryGetDto { Id = i.Id, Name = i.Name }).ToListAsync();

        var pagination = new Pagination<IndustryGetDto>
        {
            Items = industryGetDtos,
            TotalCount = totalItem,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItem,
            TotalPage = (int)Math.Ceiling((double)totalItem / pageSize),
        };

		return new BaseResponse<Pagination<IndustryGetDto>>
		{
			StatusCode = HttpStatusCode.OK,
			Message = "Industry search is successfully completed.",
			Data = pagination
		};
	}

    public async Task<BaseResponse<IndustryGetDto>> UpdateAsync(Guid id, IndustryUpdateDto industryUpdateDto)
    {
        if (id != industryUpdateDto.Id)
        {
            return new BaseResponse<IndustryGetDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Id does not match with the route parameter.",
                Data = null
            };
        }

        var validator = new IndustryUpdateValidator();
        var validationResult = await validator.ValidateAsync(industryUpdateDto);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<IndustryGetDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

        var industry = await _industryRepository.GetByIdAsync(id);
        if (industry == null || industry.IsDeleted)
        {
            return new BaseResponse<IndustryGetDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The industry does not exist."
            };
        }

        var existedIndustry = await _industryRepository.GetByFilter(
            expression: i => i.Name.ToLower() == industryUpdateDto.Name.ToLower() && i.Id != id && !i.IsDeleted,
            isTracking: false);

        if (existedIndustry != null)
        {
            return new BaseResponse<IndustryGetDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = $"An industry with the name '{industryUpdateDto.Name}' already exists."
            };
        }

        industry.Name = industryUpdateDto.Name;
        await _industryRepository.SaveChangesAsync();

        var updatedIndustryDto = _mapper.Map<IndustryGetDto>(industry);

        return new BaseResponse<IndustryGetDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "The industry has been successfully updated.",
            Data = updatedIndustryDto
        };
    }

}
