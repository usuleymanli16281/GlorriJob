using AutoMapper;
using FluentValidation;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos;
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Application.Validations.VacancyDetail;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GlorriJob.Persistence.Implementations.Services;

public class VacancyDetailService : IVacancyDetailService
{
    private readonly IVacancyDetailRepository _vacancyDetailRepository;
    private readonly IMapper _mapper;

    public VacancyDetailService(IVacancyDetailRepository vacancyDetailRepository, IMapper mapper)
    {
        _vacancyDetailRepository = vacancyDetailRepository;
        _mapper = mapper;
    }

    public async Task<VacancyDetailGetDto> CreateAsync(VacancyDetailCreateDto createVacancyDetailDto)
    {
        var validator = new VacancyDetailCreateValidator();
        var validationResult = await validator.ValidateAsync(createVacancyDetailDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existingVacancyDetail = await _vacancyDetailRepository.GetByFilter(
            expression: v => v.VacancyId == createVacancyDetailDto.VacancyId,
            isTracking: false
        );

        if (existingVacancyDetail != null)
        {
            throw new AlreadyExistsException($"A vacancy detail with the VacancyId '{createVacancyDetailDto.VacancyId}' already exists.");
        }

        var vacancyDetail = _mapper.Map<VacancyDetail>(createVacancyDetailDto);
        await _vacancyDetailRepository.AddAsync(vacancyDetail);
        await _vacancyDetailRepository.SaveChangesAsync();
        return _mapper.Map<VacancyDetailGetDto>(vacancyDetail);
    }

    public async Task<VacancyDetailUpdateDto> UpdateAsync(Guid id, VacancyDetailUpdateDto vacancyDetailUpdateDto)
    {
        if (id != vacancyDetailUpdateDto.Id)
        {
            throw new BadRequestException("Id doesn't match with the route parameter.");
        }

        var validator = new VacancyDetailUpdateValidator();
        var validationResult = await validator.ValidateAsync(vacancyDetailUpdateDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var vacancyDetail = await _vacancyDetailRepository.GetByIdAsync(id);
        if (vacancyDetail == null)
        {
            throw new NotFoundException("VacancyDetail not found.");
        }

        _mapper.Map(vacancyDetailUpdateDto, vacancyDetail);
        _vacancyDetailRepository.Update(vacancyDetail);
        await _vacancyDetailRepository.SaveChangesAsync();

        return vacancyDetailUpdateDto;
    }

    public async Task<VacancyDetailGetDto> GetByIdAsync(Guid id)
    {
        var vacancyDetail = await _vacancyDetailRepository.GetByIdAsync(id);
        if (vacancyDetail == null)
        {
            throw new NotFoundException("VacancyDetail not found.");
        }

        return _mapper.Map<VacancyDetailGetDto>(vacancyDetail);
    }

    public async Task DeleteAsync(Guid id)
    {
        var vacancyDetail = await _vacancyDetailRepository.GetByIdAsync(id);
        if (vacancyDetail == null)
        {
            throw new NotFoundException("VacancyDetail not found.");
        }

        vacancyDetail.IsDeleted = true;
        await _vacancyDetailRepository.SaveChangesAsync();
    }

    public async Task<Pagination<VacancyDetailGetDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new BadRequestException("Page number and page size should be greater than 0.");

        IQueryable<VacancyDetail> query = _vacancyDetailRepository.GetAll(v => !v.IsDeleted);

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<VacancyDetailGetDto> vacancyDetailGetDtos = await query
            .Select(v => new VacancyDetailGetDto
            {
                Id = v.Id,
                VacancyType = v.VacancyType,
                Content = v.Content,
                Salary = v.Salary,
                VacancyId = v.VacancyId
            })
            .ToListAsync();

        return new Pagination<VacancyDetailGetDto>
        {
            Items = vacancyDetailGetDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };
    }

    public async Task<Pagination<VacancyDetailGetDto>> SearchByContentAsync(string content, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new BadRequestException("Page number and page size should be greater than 0.");

        IQueryable<VacancyDetail> query = _vacancyDetailRepository.GetAll(v => !v.IsDeleted && v.Content.ToLower().Contains(content.ToLower()));

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<VacancyDetailGetDto> vacancyDetailGetDtos = await query
            .Select(v => new VacancyDetailGetDto
            {
                Id = v.Id,
                VacancyType = v.VacancyType,
                Content = v.Content,
                Salary = v.Salary,
                VacancyId = v.VacancyId
            })
            .ToListAsync();

        return new Pagination<VacancyDetailGetDto>
        {
            Items = vacancyDetailGetDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };
    }
}


