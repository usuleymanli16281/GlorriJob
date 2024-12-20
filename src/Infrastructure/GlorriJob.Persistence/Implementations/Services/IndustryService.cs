
using AutoMapper;
using FluentValidation;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Industry;
using GlorriJob.Application.Validations.Industry;
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

    public async Task<IndustryGetDto> CreateAsync(IndustryCreateDto industryCreateDto)
    {
        var validator = new IndustryCreateValidator();
        var validationResult = await validator.ValidateAsync(industryCreateDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existedIndustry = await _industryRepository.GetByFilter(
            expression: i => i.Name == industryCreateDto.Name && !i.IsDeleted,
            isTracking: false);

        if (existedIndustry is not null)
        {
            throw new AlreadyExistsException($"An industry with the name '{industryCreateDto.Name}' already exists.");
        }

        var createdIndustry = _mapper.Map<Industry>(industryCreateDto);
        await _industryRepository.AddAsync(createdIndustry);
        await _industryRepository.SaveChangesAsync();

        return _mapper.Map<IndustryGetDto>(createdIndustry);
    }

    public async Task DeleteAsync(Guid id)
    {
        var industry = await _industryRepository.GetByIdAsync(id);
        if (industry == null || industry.IsDeleted)
        {
            throw new NotFoundException("This industry does not exist.");
        }

        industry.IsDeleted = true;
        await _industryRepository.SaveChangesAsync();
    }

    public async Task<Pagination<IndustryGetDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new BadRequestException("Page number and page size should be greater than 0.");

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

        return new Pagination<IndustryGetDto>
        {
            Items = industryGetDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };
    }

    public async Task<IndustryGetDto> GetByIdAsync(Guid id)
    {
        var industry = await _industryRepository.GetByIdAsync(id);
        if (industry == null || industry.IsDeleted)
        {
            throw new NotFoundException("This industry does not exist.");
        }

        return _mapper.Map<IndustryGetDto>(industry);
    }

    public async Task<Pagination<IndustryGetDto>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new BadRequestException("Page number and page size should be greater than 0.");

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

        return new Pagination<IndustryGetDto>
        {
            Items = industryGetDtos,
            TotalCount = totalItem,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItem,
            TotalPage = (int)Math.Ceiling((double)totalItem / pageSize),
        };
    }

    public async Task<IndustryUpdateDto> UpdateAsync(Guid id, IndustryUpdateDto industryUpdateDto)
    {
        if (id != industryUpdateDto.Id)
        {
            throw new BadRequestException("Id doesn't match with the route parameter.");
        }

        var validator = new IndustryUpdateValidator();
        var validationResult = await validator.ValidateAsync(industryUpdateDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var industry = await _industryRepository.GetByIdAsync(id);
        if (industry == null || industry.IsDeleted)
        {
            throw new NotFoundException("This industry does not exist.");
        }

        var modifiedIndustry = _mapper.Map<Industry>(industryUpdateDto);
        _industryRepository.Update(modifiedIndustry);
        await _industryRepository.SaveChangesAsync();

        return industryUpdateDto;
    }
}
