
using FluentValidation;
using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Application.Validations.Category;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
namespace GlorriJob.Persistence.Implementations.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryGetDto> CreateAsync(CategoryCreateDto categoryCreateDto)
    {
        var validator = new CategoryCreateValidator();
        var validationResult = await validator.ValidateAsync(categoryCreateDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existedCategory = await _categoryRepository.GetByFilter(
            expression: c => c.Name.ToLower() == categoryCreateDto.Name.ToLower() && !c.IsDeleted,
            isTracking: false);

        if (existedCategory != null)
        {
            throw new AlreadyExistsException($"A category with the name '{categoryCreateDto.Name}' already exists.");
        }

        var createdCategory = _mapper.Map<Category>(categoryCreateDto);
        await _categoryRepository.AddAsync(createdCategory);
        await _categoryRepository.SaveChangesAsync();

        return _mapper.Map<CategoryGetDto>(createdCategory);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null || category.IsDeleted)
        {
            throw new NotFoundException("This category does not exist.");
        }

        category.IsDeleted = true;
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task<Pagination<CategoryGetDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = true)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new BadRequestException("Page number and page size should be greater than 0.");

        IQueryable<Category> query = _categoryRepository.GetAll(c => !c.IsDeleted);

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        var categoryDtos = await query
            .Select(c => new CategoryGetDto
            {
                Id = c.Id,
                Name = c.Name,
                ExistedVacancy = c.Vacancies.Count(v => !v.IsDeleted)
            })
            .ToListAsync();

        return new Pagination<CategoryGetDto>
        {
            Items = categoryDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };
    }

    public async Task<CategoryGetDto> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null || category.IsDeleted)
        {
            throw new NotFoundException("This category does not exist.");
        }

        return _mapper.Map<CategoryGetDto>(category);
    }

    public async Task<Pagination<CategoryGetDto>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = true)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new BadRequestException("Page number and page size should be greater than 0.");

        IQueryable<Category> query = _categoryRepository.GetAll(c => !c.IsDeleted && c.Name.ToLower().Contains(name.ToLower()));

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        var categoryDtos = await query
            .Select(c => new CategoryGetDto
            {
                Id = c.Id,
                Name = c.Name,
                ExistedVacancy = c.Vacancies.Count(v => !v.IsDeleted)
            })
            .ToListAsync();

        return new Pagination<CategoryGetDto>
        {
            Items = categoryDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };
    }

    public async Task<CategoryGetDto> UpdateAsync(Guid id, CategoryUpdateDto categoryUpdateDto)
    {
        if (id != categoryUpdateDto.Id)
        {
            throw new BadRequestException("Id doesn't match with the route parameter.");
        }

        var validator = new CategoryUpdateValidator();
        var validationResult = await validator.ValidateAsync(categoryUpdateDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null || category.IsDeleted)
        {
            throw new NotFoundException("This category does not exist.");
        }

        category.Name = categoryUpdateDto.Name;
        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync();

        return _mapper.Map<CategoryGetDto>(category);
    }
     
}

