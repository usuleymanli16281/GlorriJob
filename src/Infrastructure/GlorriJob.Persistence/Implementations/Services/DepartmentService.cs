using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Common.Shared;
using GlorriJob.Domain;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Implementations.Services
{
    public class DepartmentService : IDepartmentService
    {
        private IDepartmentRepository _departmentRepository { get; }
        private IMapper _mapper { get; }

        public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<BaseResponse<DepartmentGetDto>> CreateAsync(DepartmentCreateDto departmentCreateDto)
        {
            var validator = new DepartmentCreateValidator();
            var validationResult = await validator.ValidateAsync(departmentCreateDto);

            if (!validationResult.IsValid)
            {
                return new BaseResponse<DepartmentGetDto>(
                    "Validation failed: " + string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                    ResponseStatusCode.BadRequest.ToString(),
                    null
                );
            }

            var existedDepartment = await _departmentRepository.GetByFilter(
                d => !d.IsDeleted &&
                     d.Name.ToLower().Contains(departmentCreateDto.Name.ToLower()) &&
                     d.BranchId == departmentCreateDto.BranchId
            );

            if (existedDepartment is not null)
            {
                return new BaseResponse<DepartmentGetDto>(
                    "The branch already has this department.",
                    ResponseStatusCode.Conflict.ToString(),
                    null
                );
            }

            var createdDepartment = _mapper.Map<Department>(departmentCreateDto);
            await _departmentRepository.AddAsync(createdDepartment);
            await _departmentRepository.SaveChangesAsync();

            var departmentGetDto = _mapper.Map<DepartmentGetDto>(createdDepartment);

            return new BaseResponse<DepartmentGetDto>(
                "Department created successfully.",
                ResponseStatusCode.Created.ToString(),
                departmentGetDto
            );
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department is null || department.IsDeleted)
            {
                return new BaseResponse<bool>(
                    "This department does not exist.",
                    ResponseStatusCode.NotFound.ToString(),
                    false
                );
            }

            department.IsDeleted = true;
            await _departmentRepository.SaveChangesAsync();

            return new BaseResponse<bool>(
                "Department deleted successfully.",
                ResponseStatusCode.Success.ToString(),
                true
            );
        }

        public async Task<BaseResponse<Pagination<DepartmentGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return new BaseResponse<Pagination<DepartmentGetDto>>(
                    "Page number and page size should be greater than 0.",
                    ResponseStatusCode.BadRequest.ToString(),
                    null
                );
            }

            IQueryable<Department> query = _departmentRepository.GetAll(d => !d.IsDeleted);

            int totalItems = await query.CountAsync();

            if (isPaginated)
            {
                int skip = (pageNumber - 1) * pageSize;
                query = query.Skip(skip).Take(pageSize);
            }

            List<DepartmentGetDto> departmentGetDtos = await query
                .Select(d => new DepartmentGetDto { Id = d.Id, Name = d.Name })
                .ToListAsync();

            var pagination = new Pagination<DepartmentGetDto>
            {
                Items = departmentGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            };

            return new BaseResponse<Pagination<DepartmentGetDto>>(
                "Departments fetched successfully.",
                ResponseStatusCode.Success.ToString(),
                pagination
            );
        }

        public async Task<BaseResponse<DepartmentGetDto>> GetByIdAsync(Guid id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department is null || department.IsDeleted)
            {
                return new BaseResponse<DepartmentGetDto>(
                    "This department does not exist.",
                    ResponseStatusCode.NotFound.ToString(),
                    null
                );
            }

            var departmentGetDto = _mapper.Map<DepartmentGetDto>(department);

            return new BaseResponse<DepartmentGetDto>(
                "Department fetched successfully.",
                ResponseStatusCode.Success.ToString(),
                departmentGetDto
            );
        }

        public async Task<BaseResponse<Pagination<DepartmentGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return new BaseResponse<Pagination<DepartmentGetDto>>(
                    "Page number and page size should be greater than 0.",
                    ResponseStatusCode.BadRequest.ToString(),
                    null
                );
            }

            IQueryable<Department> query = _departmentRepository.GetAll(d => !d.IsDeleted && d.Name.ToLower().Contains(name.ToLower()));

            int totalItems = await query.CountAsync();

            if (isPaginated)
            {
                int skip = (pageNumber - 1) * pageSize;
                query = query.Skip(skip).Take(pageSize);
            }

            List<DepartmentGetDto> departmentGetDtos = await query
                .Select(d => new DepartmentGetDto { Id = d.Id, Name = d.Name })
                .ToListAsync();

            var pagination = new Pagination<DepartmentGetDto>
            {
                Items = departmentGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            };

            return new BaseResponse<Pagination<DepartmentGetDto>>(
                "Department search completed successfully.",
                ResponseStatusCode.Success.ToString(),
                pagination
            );
        }

        public async Task<BaseResponse<DepartmentUpdateDto>> UpdateAsync(Guid id, DepartmentUpdateDto departmentUpdateDto)
        {
            if (id != departmentUpdateDto.Id)
            {
                return new BaseResponse<DepartmentUpdateDto>(
                    "Id doesn't match with the root",
                    ResponseStatusCode.BadRequest.ToString(),
                    null
                );
            }

            var validator = new DepartmentUpdateValidator();
            var validationResult = await validator.ValidateAsync(departmentUpdateDto);

            if (!validationResult.IsValid)
            {
                return new BaseResponse<DepartmentUpdateDto>(
                    "Validation failed: " + string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                    ResponseStatusCode.BadRequest.ToString(),
                    null
                );
            }

            var department = await _departmentRepository.GetByIdAsync(id);
            if (department is null || department.IsDeleted)
            {
                return new BaseResponse<DepartmentUpdateDto>(
                    "This department does not exist.",
                    ResponseStatusCode.NotFound.ToString(),
                    null
                );
            }

            department.Name = departmentUpdateDto.Name;
            _departmentRepository.Update(department);
            await _departmentRepository.SaveChangesAsync();

            var updatedDepartmentDto = _mapper.Map<DepartmentUpdateDto>(department);

            return new BaseResponse<DepartmentUpdateDto>(
                "Department updated successfully.",
                ResponseStatusCode.Success.ToString(),
                updatedDepartmentDto
            );
        }
    }

}
