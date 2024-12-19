using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Department;
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
		private IDepartmentRepository _departmentRepository {  get; }
		private IMapper _mapper { get; }
        public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
			_mapper = mapper;
        }
        public async Task<DepartmentGetDto> CreateAsync(DepartmentCreateDto departmentCreateDto)
		{
			var existedDepartment = _departmentRepository.GetByFilter(
				d => !d.IsDeleted &&
				d.Name.ToLower().Contains(departmentCreateDto.Name.ToLower()) &&
				d.BranchId == departmentCreateDto.BranchId);
			if(existedDepartment is not null)
			{
				throw new AlreadyExistsException($"The branch already has this department.");
			}
			var createdDepartment = _mapper.Map<Department>(departmentCreateDto);
			await _departmentRepository.AddAsync(createdDepartment);
			await _departmentRepository.SaveChangesAsync();
			return _mapper.Map<DepartmentGetDto>(createdDepartment);
		}

		public async Task DeleteAsync(Guid id)
		{
			var department = await _departmentRepository.GetByIdAsync(id);
			if (department is null || department.IsDeleted)
			{
				throw new NotFoundException("This department does not exist");
			}
			department.IsDeleted = true;
			await _departmentRepository.SaveChangesAsync();
		}

		public async Task<Pagination<DepartmentGetDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			if (pageNumber < 1 || pageSize < 1)
				throw new BadRequestException("Page number and page size should be greater than 0.");
			IQueryable<Department> query = _departmentRepository.GetAll(d => !d.IsDeleted);
			int totalItems = await query.CountAsync();
			if (isPaginated)
			{
				int skip = (pageNumber - 1) * pageSize;
				query = query.Skip(skip).Take(pageSize);
			}
			List<DepartmentGetDto> departmentGetDtos = await query.Select(d => new DepartmentGetDto { Id = d.Id, Name = d.Name }).ToListAsync();
			return new Pagination<DepartmentGetDto>
			{
				Items = departmentGetDtos,
				TotalCount = totalItems,
				PageIndex = pageNumber,
				PageSize = isPaginated ? pageSize : totalItems,
				TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
			};
		}

		public async Task<DepartmentGetDto> GetByIdAsync(Guid id)
		{
			var department = await _departmentRepository.GetByIdAsync(id);
			if (department is null || department.IsDeleted)
			{
				throw new NotFoundException("This department does not exist");
			}
			var departmentGetDto = _mapper.Map<DepartmentGetDto>(department);
			return departmentGetDto;
		}

		public async Task<Pagination<DepartmentGetDto>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			if (pageNumber < 1 || pageSize < 1)
				throw new BadRequestException("Page number and page size should be greater than 0.");
			IQueryable<Department> query = _departmentRepository.GetAll(d => !d.IsDeleted && d.Name.ToLower().Contains(name.ToLower()));
			int totalItems = await query.CountAsync();
			if (isPaginated)
			{
				int skip = (pageNumber - 1) * pageSize;
				query = query.Skip(skip).Take(pageSize);
			}
			List<DepartmentGetDto> departmentGetDtos = await query.Select(d => new DepartmentGetDto { Id = d.Id, Name = d.Name }).ToListAsync();
			return new Pagination<DepartmentGetDto>
			{
				Items = departmentGetDtos,
				TotalCount = totalItems,
				PageIndex = pageNumber,
				PageSize = isPaginated ? pageSize : totalItems,
				TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
			};
		}

		public async Task<DepartmentUpdateDto> UpdateAsync(Guid id, DepartmentUpdateDto departmentUpdateDto)
		{
			if (id != departmentUpdateDto.Id)
			{
				throw new BadRequestException("Id doesn't match with the root");
			}
			var department = await _departmentRepository.GetByIdAsync(id);
			if(department is null || department.IsDeleted)
			{
				throw new NotFoundException("This department does not exist");
			}
			var modifiedDepartment = _mapper.Map<Department>(departmentUpdateDto);
			_departmentRepository.Update(modifiedDepartment);
			await _departmentRepository.SaveChangesAsync();
			return departmentUpdateDto;
		}
	}
}
