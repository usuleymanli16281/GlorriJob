using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Company;
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
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Implementations.Services
{
	public class DepartmentService : IDepartmentService
	{
		private IDepartmentRepository _departmentRepository { get; }
		private IBranchRepository _branchRepository { get; }
		private IMapper _mapper { get; }

		public DepartmentService(IDepartmentRepository departmentRepository, IBranchRepository branchRepository, IMapper mapper)
		{
			_departmentRepository = departmentRepository;
			_branchRepository = branchRepository; 
			_mapper = mapper;
		}

		public async Task<BaseResponse<object>> CreateAsync(DepartmentCreateDto departmentCreateDto)
		{
			var validator = new DepartmentCreateValidator();
			var validationResult = await validator.ValidateAsync(departmentCreateDto);

			if (!validationResult.IsValid)
			{
				new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
				};
			}
			var branch  = await _branchRepository.GetByIdAsync(departmentCreateDto.BranchId);
			if(branch is null)
			{
				new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The branch does not exist"
				};
			}
			var existedDepartment = await _departmentRepository.GetByFilter(
				d => !d.IsDeleted &&
					 d.Name.ToLower() == departmentCreateDto.Name.ToLower() &&
					 d.BranchId == departmentCreateDto.BranchId
			);

			if (existedDepartment is not null)
			{
				new BaseResponse<DepartmentGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "This department already exists in the branch"
				};
			}

			var createdDepartment = _mapper.Map<Department>(departmentCreateDto);
			await _departmentRepository.AddAsync(createdDepartment);
			await _departmentRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.Created,
				Message = "The department is successfully created."
			};
		}

		public async Task<BaseResponse<object>> DeleteAsync(Guid id)
		{
			var department = await _departmentRepository.GetByIdAsync(id);
			if (department is null || department.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The department does not exist."
				};
			}
			department.IsDeleted = true;
			await _departmentRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NoContent,
				Message = "The department is successfully deleted."
			};
		}

		public async Task<BaseResponse<Pagination<DepartmentGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			if (pageNumber < 1 || pageSize < 1)
			{
				return new BaseResponse<Pagination<DepartmentGetDto>>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Page number and page size should be greater than 0.",
					Data = null
				};
			}

			IQueryable<Department> query = _departmentRepository.GetAll(d => !d.IsDeleted);

			int totalItems = await query.CountAsync();
			if (totalItems == 0)
			{
				return new BaseResponse<Pagination<DepartmentGetDto>>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The department does not exist"
				};
			}

			if (isPaginated)
			{
				int skip = (pageNumber - 1) * pageSize;
				query = query.Skip(skip).Take(pageSize);
			}

			List<DepartmentGetDto> departmentGetDtos = await query
				.Select(d => new DepartmentGetDto { Id = d.Id, Name = d.Name, BranchId = d.BranchId })
				.ToListAsync();

			var pagination = new Pagination<DepartmentGetDto>
			{
				Items = departmentGetDtos,
				TotalCount = totalItems,
				PageIndex = pageNumber,
				PageSize = isPaginated ? pageSize : totalItems,
				TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
			};

			return new BaseResponse<Pagination<DepartmentGetDto>>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "Departments are successfully fetched.",
				Data = pagination
			};
		}

		public async Task<BaseResponse<DepartmentGetDto>> GetByIdAsync(Guid id)
		{
			var department = await _departmentRepository.GetByIdAsync(id);
			if (department is null || department.IsDeleted)
			{
				return new BaseResponse<DepartmentGetDto>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The department does not exist."
				};
			}

			var departmentGetDto = _mapper.Map<DepartmentGetDto>(department);

			return new BaseResponse<DepartmentGetDto>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The department is successfully retrieved.",
				Data = departmentGetDto
			};
		}

		public async Task<BaseResponse<Pagination<DepartmentGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			if (pageNumber < 1 || pageSize < 1)
			{
				return new BaseResponse<Pagination<DepartmentGetDto>>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Page number and page size should be greater than 0."
				};
			}

			IQueryable<Department> query = _departmentRepository.GetAll(d => !d.IsDeleted && d.Name.ToLower().Contains(name.ToLower()));

			int totalItems = await query.CountAsync();
			if (totalItems == 0)
			{
				return new BaseResponse<Pagination<DepartmentGetDto>>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The department does not exist"
				};
			}
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
			return new BaseResponse<Pagination<DepartmentGetDto>>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The department search is successfully completed.",
				Data = pagination
			};
		}

		public async Task<BaseResponse<object>> UpdateAsync(Guid id, DepartmentUpdateDto departmentUpdateDto)
		{
			if (id != departmentUpdateDto.Id)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Id does not match with the root.",
					Data = null
				};
			}

			var validator = new DepartmentUpdateValidator();
			var validationResult = await validator.ValidateAsync(departmentUpdateDto);

			if (!validationResult.IsValid)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
				};
			}

			var department = await _departmentRepository.GetByIdAsync(id);
			if (department is null || department.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "The department does not exist."
				};
			}
			var branch = await _branchRepository.GetByIdAsync(departmentUpdateDto.BranchId);
			if (branch is null)
			{
				new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The branch does not exist"
				};
			}
			var existingDepartment = await _departmentRepository.GetByFilter(
			expression: d => department.Name.ToLower() != departmentUpdateDto.Name.ToLower() && 
			d.Name.ToLower() == departmentUpdateDto.Name.ToLower() && 
			d.BranchId == departmentUpdateDto.BranchId &&
			d.Id != id && 
			!d.IsDeleted,
			isTracking: false);

			if (existingDepartment is not null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"A department with the name '{departmentUpdateDto.Name}' already exists in the branch."
				};
			}

			department.Name = departmentUpdateDto.Name;
			_departmentRepository.Update(department);
			await _departmentRepository.SaveChangesAsync();

			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The department is successfully updated."
			};

		}
	}

}
