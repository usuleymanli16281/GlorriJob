using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface ICategoryService
{
    Task<CategoryGetDto> GetByIdAsync(Guid id);
    Task<Pagination<CategoryGetDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = true);
    Task<Pagination<CategoryGetDto>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = true);
    Task<CategoryGetDto> CreateAsync(CategoryCreateDto createCategoryDto);
    Task<CategoryGetDto> UpdateAsync(Guid id, CategoryUpdateDto categoryUpdateDto);
    Task DeleteAsync(Guid id);
}
