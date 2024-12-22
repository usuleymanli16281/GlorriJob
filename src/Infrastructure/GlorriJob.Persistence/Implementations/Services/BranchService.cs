using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Branch;
using GlorriJob.Application.Validations.Branch;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GlorriJob.Persistence.Implementations.Services;

public class BranchService : IBranchService
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public BranchService(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<BranchGetDto> CreateAsync(BranchCreateDto createBranchDto)
    {
        var validator = new BranchCreateValidator();
        var validationResult = await validator.ValidateAsync(createBranchDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existingBranch = await _branchRepository.GetByFilter(
            expression: b => b.Name == createBranchDto.Name && b.CompanyId == createBranchDto.CompanyId && !b.IsDeleted,
            isTracking: false);

        if (existingBranch != null)
        {
            throw new AlreadyExistsException($"A branch with the name '{createBranchDto.Name}' already exists for this company.");
        }

        var branch = _mapper.Map<Branch>(createBranchDto);
        await _branchRepository.AddAsync(branch);
        await _branchRepository.SaveChangesAsync();

        return _mapper.Map<BranchGetDto>(branch);
    }

    public async Task<BranchGetDto> UpdateAsync(Guid id, BranchUpdateDto branchUpdateDto)
    {
        if (id != branchUpdateDto.Id)
        {
            throw new BadRequestException("Id doesn't match with the route parameter.");
        }

        var validator = new BranchUpdateValidator();
        var validationResult = await validator.ValidateAsync(branchUpdateDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var branch = await _branchRepository.GetByIdAsync(id);
        if (branch == null || branch.IsDeleted)
        {
            throw new NotFoundException("This branch does not exist.");
        }

        var modifiedBranch = _mapper.Map<Branch>(branchUpdateDto);
        _branchRepository.Update(modifiedBranch);
        await _branchRepository.SaveChangesAsync();

        return _mapper.Map<BranchGetDto>(modifiedBranch);
    }

    public async Task DeleteAsync(Guid id)
    {
        var branch = await _branchRepository.GetByIdAsync(id);
        if (branch == null || branch.IsDeleted)
        {
            throw new NotFoundException("This branch does not exist.");
        }

        branch.IsDeleted = true;
        await _branchRepository.SaveChangesAsync();
    }

    public async Task<BranchGetDto> GetByIdAsync(Guid id)
    {
        var branch = await _branchRepository.GetByIdAsync(id);
        if (branch == null || branch.IsDeleted)
        {
            throw new NotFoundException("This branch does not exist.");
        }

        return _mapper.Map<BranchGetDto>(branch);
    }

    public async Task<Pagination<BranchGetDto>> GetAllAsync(BranchFilterDto filterDto, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new BadRequestException("Page number and page size should be greater than 0.");
        }

        IQueryable<Branch> query = _branchRepository.GetAll(b => !b.IsDeleted);

        if (filterDto.IsMain.HasValue)
        {
            query = query.Where(b => b.IsMain == filterDto.IsMain.Value);
        }

        if (filterDto.CityId.HasValue)
        {
            query = query.Where(b => b.CityId == filterDto.CityId.Value);
        }

        if (filterDto.CompanyId.HasValue)
        {
            query = query.Where(b => b.CompanyId == filterDto.CompanyId.Value);
        }

        if (filterDto.DepartmentIds != null && filterDto.DepartmentIds.Any())
        {
            query = query.Where(b => b.Departments.Any(d => filterDto.DepartmentIds.Contains(d.Id)));
        }

        if (filterDto.VacancyIds != null && filterDto.VacancyIds.Any())
        {
            query = query.Where(b => b.Vacancies.Any(v => filterDto.VacancyIds.Contains(v.Id)));
        }

        int totalItems = await query.CountAsync();

        int skip = (pageNumber - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);

        List<BranchGetDto> branchDtos = await query
            .Select(b => _mapper.Map<BranchGetDto>(b))
            .ToListAsync();

        return new Pagination<BranchGetDto>
        {
            Items = branchDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = pageSize,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize)
        };
    }

    public async Task<Pagination<BranchGetDto>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new BadRequestException("Page number and page size should be greater than 0.");
        }

        IQueryable<Branch> query = _branchRepository.GetAll(b => !b.IsDeleted && b.Name.ToLower().Contains(name.ToLower()));

        int totalItems = await query.CountAsync();

        int skip = (pageNumber - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);

        List<BranchGetDto> branchDtos = await query
            .Select(b => _mapper.Map<BranchGetDto>(b))
            .ToListAsync();

        return new Pagination<BranchGetDto>
        {
            Items = branchDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = pageSize,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize)
        };
    }
}

