using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Department;
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
		Task<DepartmentGetDto> GetByIdAsync(Guid id);
		Task<Pagination<DepartmentGetDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
		Task<Pagination<DepartmentGetDto>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
		Task<DepartmentGetDto> CreateAsync(DepartmentCreateDto departmentCreateDto);
		Task<DepartmentUpdateDto> UpdateAsync(Guid id, DepartmentUpdateDto departmentUpdateDto);
		Task DeleteAsync(Guid id);
	}
}
