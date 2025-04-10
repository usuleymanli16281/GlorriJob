﻿
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
using GlorriJob.Common.Shared;
using System.Net;
using GlorriJob.Application.Dtos.Branch;
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

    public async Task<BaseResponse<object>> CreateAsync(CategoryCreateDto categoryCreateDto)
    {
        var validator = new CategoryCreateValidator();
        var validationResult = await validator.ValidateAsync(categoryCreateDto);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var existedCategory = await _categoryRepository.GetByFilter(
            expression: c => c.Name.ToLower() == categoryCreateDto.Name.ToLower() && !c.IsDeleted,
            isTracking: false);

        if (existedCategory is not null)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = $"A category with the name '{categoryCreateDto.Name}' already exists.",
                Data = null
            };
        }

        var createdCategory = _mapper.Map<Category>(categoryCreateDto);
        await _categoryRepository.AddAsync(createdCategory);
        await _categoryRepository.SaveChangesAsync();

        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.Created,
            Message = "The category is successfully created."
        };
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null || category.IsDeleted)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The category does not exist."
            };
        }

        category.IsDeleted = true;
        await _categoryRepository.SaveChangesAsync();

        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.NoContent,
            Message = "The category is successfully deleted."
        };
    }

    public async Task<BaseResponse<Pagination<CategoryGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = true)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<CategoryGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<Category> query = _categoryRepository.GetAll(c => !c.IsDeleted);

        int totalItems = await query.CountAsync();
		if (totalItems == 0)
		{
			return new BaseResponse<Pagination<CategoryGetDto>>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "The category does not exist"
			};
		}
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
                ExistedVacancy = c.ExistedVacancy
            })
            .ToListAsync();

        var pagination = new Pagination<CategoryGetDto>
        {
            Items = categoryDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };

        return new BaseResponse<Pagination<CategoryGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Categories are successfully fetched.",
            Data = pagination
        };
    }

    public async Task<BaseResponse<CategoryGetDto>> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null || category.IsDeleted)
        {
            return new BaseResponse<CategoryGetDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The category does not exist."
            };
        }

        var categoryGetDto = _mapper.Map<CategoryGetDto>(category);

        return new BaseResponse<CategoryGetDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "The category is successfully fetched.",
            Data = categoryGetDto
        };
    }

    public async Task<BaseResponse<Pagination<CategoryGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = true)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<CategoryGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0."
            };
        }

        IQueryable<Category> query = _categoryRepository.GetAll(c => !c.IsDeleted && c.Name.ToLower().Contains(name.ToLower()));

        int totalItems = await query.CountAsync();
		if (totalItems == 0)
		{
			return new BaseResponse<Pagination<CategoryGetDto>>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "The branch does not exist"
			};
		}
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
                ExistedVacancy = c.ExistedVacancy
            })
            .ToListAsync();

        var pagination = new Pagination<CategoryGetDto>
        {
            Items = categoryDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = isPaginated ? pageSize : totalItems,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
        };

        return new BaseResponse<Pagination<CategoryGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Category search is successfully completed.",
            Data = pagination
        };
    }

    public async Task<BaseResponse<object>> UpdateAsync(Guid id, CategoryUpdateDto categoryUpdateDto)
    {
        if (id != categoryUpdateDto.Id)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Id does not match with the route parameter."
            };
        }

        var validator = new CategoryUpdateValidator();
        var validationResult = await validator.ValidateAsync(categoryUpdateDto);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null || category.IsDeleted)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The category does not exist."
            };
        }

        var existedCategory = await _categoryRepository.GetByFilter(
            expression: c => category.Name.ToLower() != categoryUpdateDto.Name.ToLower() && c.Name.ToLower() == categoryUpdateDto.Name.ToLower() && c.Id != id && !c.IsDeleted,
            isTracking: false);

        if (existedCategory is not null)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = $"A category with the name '{categoryUpdateDto.Name}' already exists."
            };
        }

        category.Name = categoryUpdateDto.Name;
        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync();


        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.NoContent,
            Message = "The category has been successfully updated."
        };
    }

}


