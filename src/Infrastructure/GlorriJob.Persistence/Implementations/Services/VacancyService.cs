

using AutoMapper;
using FluentValidation;

using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos;
using GlorriJob.Application.Dtos.Branch;
using GlorriJob.Application.Dtos.Vacancy;
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Application.Validations.Vacancy;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GlorriJob.Persistence.Implementations.Services;

public class VacancyService : IVacancyService
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IMapper _mapper;

    public VacancyService(IVacancyRepository vacancyRepository, IMapper mapper)
    {
        _vacancyRepository = vacancyRepository;
        _mapper = mapper;
    }

    public async Task<VacancyGetDto> GetByIdAsync(Guid id)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(id);
        if (vacancy == null || vacancy.IsDeleted)
        {
            throw new NotFoundException("This vacancy does not exist.");
        }

        return _mapper.Map<VacancyGetDto>(vacancy);
    }

    public async Task<Pagination<VacancyGetDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new BadRequestException("Page number and page size should be greater than 0.");

        IQueryable<Vacancy> query = _vacancyRepository.GetAll(v => !v.IsDeleted);

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<VacancyGetDto> vacancyGetDtos = await query
            .Select(v => new VacancyGetDto
            {
                Id = v.Id,
                Title = v.Title,
                VacancyType = v.VacancyType,
                JobLevel = v.JobLevel,
                IsSalaryVisible = v.IsSalaryVisible,
                IsRemote = v.IsRemote,
                ExpireDate = v.ExpireDate,
                CategoryId = v.CategoryId,
                CategoryName = v.Category.Name,
                DepartmentId = v.DepartmentId,
                DepartmentName = v.Department.Name,
                CityId = v.CityId,
                CityName = v.City.Name,
                CompanyId = v.CompanyId,
                CompanyName = v.Company.Name,
                Branch = _mapper.Map<BranchUpdateDto>(v.Branch),
                VacancyDetail = _mapper.Map<VacancyDetailGetDto>(v.VacancyDetail),
            })
            .ToListAsync();

        return new Pagination<VacancyGetDto>
        {
            Items = vacancyGetDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };
    }

    public async Task<Pagination<VacancyGetDto>> SearchByNameAsync(string title, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new BadRequestException("Page number and page size should be greater than 0.");

        IQueryable<Vacancy> query = _vacancyRepository.GetAll(v => !v.IsDeleted && v.Title.ToLower().Contains(title.ToLower()));

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<VacancyGetDto> vacancyGetDtos = await query
            .Select(v => new VacancyGetDto
            {
                Id = v.Id,
                Title = v.Title,
                VacancyType = v.VacancyType,
                JobLevel = v.JobLevel,
                IsSalaryVisible = v.IsSalaryVisible,
                IsRemote = v.IsRemote,
                ExpireDate = v.ExpireDate,
                CategoryId = v.CategoryId,
                CategoryName = v.Category.Name,
                DepartmentId = v.DepartmentId,
                DepartmentName = v.Department.Name,
                CityId = v.CityId,
                CityName = v.City.Name,
                CompanyId = v.CompanyId,
                CompanyName = v.Company.Name,
                Branch = _mapper.Map<BranchUpdateDto>(v.Branch),
                VacancyDetail = _mapper.Map<VacancyDetailGetDto>(v.VacancyDetail),
            })
            .ToListAsync();

        return new Pagination<VacancyGetDto>
        {
            Items = vacancyGetDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };
    }

    public async Task<VacancyGetDto> CreateAsync(VacancyCreateDto createVacancyDto)
    {
        var validator = new VacancyCreateValidator();
        var validationResult = await validator.ValidateAsync(createVacancyDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existedVacancy = await _vacancyRepository.GetByFilter(v =>
            v.Title == createVacancyDto.Title && v.ExpireDate > DateTime.Now && !v.IsDeleted,
            isTracking: false);

        if (existedVacancy != null)
        {
            throw new AlreadyExistsException($"A vacancy with the title '{createVacancyDto.Title}' already exists.");
        }

        var createdVacancy = _mapper.Map<Vacancy>(createVacancyDto);
        await _vacancyRepository.AddAsync(createdVacancy);
        await _vacancyRepository.SaveChangesAsync();

        return _mapper.Map<VacancyGetDto>(createdVacancy);
    }

    public async Task<VacancyUpdateDto> UpdateAsync(Guid id, VacancyUpdateDto vacancyUpdateDto)
    {
        if (id != vacancyUpdateDto.Id)
        {
            throw new BadRequestException("Id doesn't match with the route parameter.");
        }

        var validator = new VacancyUpdateValidator();
        var validationResult = await validator.ValidateAsync(vacancyUpdateDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var vacancy = await _vacancyRepository.GetByIdAsync(id);
        if (vacancy == null || vacancy.IsDeleted)
        {
            throw new NotFoundException("This vacancy does not exist.");
        }

        var modifiedVacancy = _mapper.Map<Vacancy>(vacancyUpdateDto);
        _vacancyRepository.Update(modifiedVacancy);
        await _vacancyRepository.SaveChangesAsync();

        return vacancyUpdateDto;
    }

    public async Task DeleteAsync(Guid id)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(id);
        if (vacancy == null || vacancy.IsDeleted)
        {
            throw new NotFoundException("This vacancy does not exist.");
        }

        vacancy.IsDeleted = true;
        await _vacancyRepository.SaveChangesAsync();
    }

    public async Task<Pagination<VacancyGetDto>> FilterVacanciesAsync(VacancyFilterDto filterDto, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new BadRequestException("Page number and page size should be greater than 0.");

        IQueryable<Vacancy> query = _vacancyRepository.GetAll(v => !v.IsDeleted);

        if (filterDto.VacancyType.HasValue)
            query = query.Where(v => v.VacancyType == filterDto.VacancyType);

        if (filterDto.JobLevel.HasValue)
            query = query.Where(v => v.JobLevel == filterDto.JobLevel);

        if (filterDto.CategoryId.HasValue)
            query = query.Where(v => v.CategoryId == filterDto.CategoryId);

        if (filterDto.CompanyId.HasValue)
            query = query.Where(v => v.CompanyId == filterDto.CompanyId);

        if (filterDto.BranchId.HasValue)
            query = query.Where(v => v.BranchId == filterDto.BranchId);

        if (filterDto.DepartmentId.HasValue)
            query = query.Where(v => v.DepartmentId == filterDto.DepartmentId);

        if (filterDto.CityId.HasValue)
            query = query.Where(v => v.CityId == filterDto.CityId);

        if (filterDto.ExpireDate.HasValue)
            query = query.Where(v => v.ExpireDate <= filterDto.ExpireDate);

        if (filterDto.IsRemote.HasValue)
            query = query.Where(v => v.IsRemote == filterDto.IsRemote);

        if (filterDto.IsSalaryVisible.HasValue)
            query = query.Where(v => v.IsSalaryVisible == filterDto.IsSalaryVisible);

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<VacancyGetDto> vacancyGetDtos = await query
            .Select(v => new VacancyGetDto
            {
                Id = v.Id,
                Title = v.Title,
                VacancyType = v.VacancyType,
                JobLevel = v.JobLevel,
                IsSalaryVisible = v.IsSalaryVisible,
                IsRemote = v.IsRemote,
                ExpireDate = v.ExpireDate,
                CategoryId = v.CategoryId,
                CategoryName = v.Category.Name,
                DepartmentId = v.DepartmentId,
                DepartmentName = v.Department.Name,
                CityId = v.CityId,
                CityName = v.City.Name,
                CompanyId = v.CompanyId,
                CompanyName = v.Company.Name,
                Branch = _mapper.Map<BranchUpdateDto>(v.Branch),
                VacancyDetail = _mapper.Map<VacancyDetailGetDto>(v.VacancyDetail),
            })
            .ToListAsync();

        return new Pagination<VacancyGetDto>
        {
            Items = vacancyGetDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };
    }
}



