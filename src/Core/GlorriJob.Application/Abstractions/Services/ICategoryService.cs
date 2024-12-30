using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface ICategoryService
{
    Task<BaseResponse<CategoryGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<Pagination<CategoryGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = true);
    Task<BaseResponse<Pagination<CategoryGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = true);
    Task<BaseResponse<CategoryGetDto>> CreateAsync(CategoryCreateDto createCategoryDto);
    Task<BaseResponse<CategoryGetDto>> UpdateAsync(Guid id, CategoryUpdateDto categoryUpdateDto);
    Task<BaseResponse<object>> DeleteAsync(Guid id);
}

