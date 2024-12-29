using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Abstractions.Services
{
    public interface IDepartmentService
	{
        Task<ApiResponse<DepartmentGetDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<Pagination<DepartmentGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
        Task<ApiResponse<Pagination<DepartmentGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
        Task<ApiResponse<DepartmentGetDto>> CreateAsync(DepartmentCreateDto departmentCreateDto);
        Task<ApiResponse<DepartmentUpdateDto>> UpdateAsync(Guid id, DepartmentUpdateDto departmentUpdateDto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
	}
}
