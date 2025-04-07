
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;


namespace GlorriJob.Application.Abstractions.Services
{
    public interface IDepartmentService
	{
        Task<BaseResponse<DepartmentGetDto>> GetByIdAsync(Guid id);
        Task<BaseResponse<Pagination<DepartmentGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
        Task<BaseResponse<Pagination<DepartmentGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
        Task<BaseResponse<object>> CreateAsync(DepartmentCreateDto departmentCreateDto);
        Task<BaseResponse<object>> UpdateAsync(Guid id, DepartmentUpdateDto departmentUpdateDto);
        Task<BaseResponse<object>> DeleteAsync(Guid id);
	}
}
