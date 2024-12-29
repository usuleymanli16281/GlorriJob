
using AutoMapper;
using FluentValidation;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Industry;
using GlorriJob.Application.Validations.Industry;
using GlorriJob.Common.Shared;
using GlorriJob.Domain;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


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

    public async Task<ApiResponse<IndustryGetDto>> CreateAsync(IndustryCreateDto industryCreateDto)
    {
        var validator = new IndustryCreateValidator();
        var validationResult = await validator.ValidateAsync(industryCreateDto);

        if (!validationResult.IsValid)
        {
            return new ApiResponse<IndustryGetDto>(
             "Validation failed: " + string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
             ResponseStatusCode.BadRequest.ToString(),
             null
         );

        }

        var existedIndustry = await _industryRepository.GetByFilter(
            expression: i => i.Name == industryCreateDto.Name && !i.IsDeleted,
            isTracking: false);

        if (existedIndustry is not null)
        {
            return new ApiResponse<IndustryGetDto>(
                $"An industry with the name '{industryCreateDto.Name}' already exists.", 
                ResponseStatusCode.Conflict.ToString(), 
                null
                );
        }

        var createdIndustry = _mapper.Map<Industry>(industryCreateDto);
        await _industryRepository.AddAsync(createdIndustry);
        await _industryRepository.SaveChangesAsync();

        var industryGetDto = _mapper.Map<IndustryGetDto>(createdIndustry);

        return new ApiResponse<IndustryGetDto>(
            "Industry created successfully.", 
            ResponseStatusCode.Created.ToString(), 
            industryGetDto
            );

    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var industry = await _industryRepository.GetByIdAsync(id);
        if (industry is null || industry.IsDeleted)
        {
            return new ApiResponse<bool>(
                "This industry does not exist.", 
                ResponseStatusCode.NotFound.ToString(), 
                false);

        }
        industry.IsDeleted = true;
        await _industryRepository.SaveChangesAsync();

        return new ApiResponse<bool>(
            "Industry deleted successfully.", 
            ResponseStatusCode.Success.ToString(), 
            true);
    }


    public async Task<ApiResponse<Pagination<IndustryGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
            return new ApiResponse<Pagination<IndustryGetDto>>(
                "Page number and page size should be greater than 0.", 
                ResponseStatusCode.BadRequest.ToString(), 
                null);

        IQueryable<Industry> query = _industryRepository.GetAll(i => !i.IsDeleted);

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<IndustryGetDto> industryGetDtos = await query
     .Select(i => new IndustryGetDto { Id = i.Id, Name = i.Name })
     .ToListAsync();

        var pagination = new Pagination<IndustryGetDto>
        {
            Items = industryGetDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };

        return new ApiResponse<Pagination<IndustryGetDto>>(
            "Industries fetched successfully.", 
            ResponseStatusCode.Success.ToString(), 
            pagination);
    }


    public async Task<ApiResponse<IndustryGetDto>> GetByIdAsync(Guid id)
    {
        var industry = await _industryRepository.GetByIdAsync(id);
        if (industry is null || industry.IsDeleted)
        {
            return new ApiResponse<IndustryGetDto>("This industry does not exist.", ResponseStatusCode.NotFound.ToString(), null);
        }

        var industryGetDto = _mapper.Map<IndustryGetDto>(industry);
        return new ApiResponse<IndustryGetDto>("Industry fetched successfully.", ResponseStatusCode.Success.ToString(), industryGetDto);
    }

    public async Task<ApiResponse<Pagination<IndustryGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
            return new ApiResponse<Pagination<IndustryGetDto>>(
                "Page number and page size should be greater than 0.", 
                ResponseStatusCode.BadRequest.ToString(), 
                null);

        IQueryable<Industry> query = _industryRepository.GetAll(i => !i.IsDeleted && i.Name.ToLower().Contains(name.ToLower()));

        int totalItem = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<IndustryGetDto> industryGetDtos = await query
            .Select(i => new IndustryGetDto { Id = i.Id, Name = i.Name })
            .ToListAsync();

        var pagination = new Pagination<IndustryGetDto>
        {
            Items = industryGetDtos,
            TotalCount = totalItem,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItem,
            TotalPage = (int)Math.Ceiling((double)totalItem / pageSize),
        };

        return new ApiResponse<Pagination<IndustryGetDto>>(
            "Industries search completed successfully.", 
            ResponseStatusCode.Success.ToString(), 
            pagination);
    }

    public async Task<ApiResponse<IndustryUpdateDto>> UpdateAsync(Guid id, IndustryUpdateDto industryUpdateDto)
    {
        if (id != industryUpdateDto.Id)
        {
            return new ApiResponse<IndustryUpdateDto>(
            "The ID provided does not match the ID in the URL parameter.",
            ResponseStatusCode.BadRequest.ToString(),
            null
        );
        }

        var validator = new IndustryUpdateValidator();
        var validationResult = await validator.ValidateAsync(industryUpdateDto);

        if (!validationResult.IsValid)
        {
            return new ApiResponse<IndustryUpdateDto>(
            "Validation failed: " + string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
            ResponseStatusCode.BadRequest.ToString(),
            null
        );
        }

        var industry = await _industryRepository.GetByIdAsync(id);
        if (industry is null || industry.IsDeleted)
        {
            return new ApiResponse<IndustryUpdateDto>(
             "The requested industry does not exist or is deleted.",
             ResponseStatusCode.NotFound.ToString(),
             null
         );
        }


        industry.Name = industryUpdateDto.Name;
        _industryRepository.Update(industry);
        await _industryRepository.SaveChangesAsync();

        var updatedIndustryDto = _mapper.Map<IndustryUpdateDto>(industry);

        return new ApiResponse<IndustryUpdateDto>(
         "Industry updated successfully.",
         ResponseStatusCode.Success.ToString(),
         updatedIndustryDto
     );

    }
}
