using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Branch;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Industry;
using GlorriJob.Application.Validations.Branch;
using GlorriJob.Application.Validations.Category;
using GlorriJob.Application.Validations.City;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Implementations.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Implementations.Services
{
	public class BranchService : IBranchService
	{
		private readonly IBranchRepository _branchRepository;
		private readonly ICompanyRepository _companyRepository;
		private readonly ICityRepository _cityRepository;
		private readonly IMapper _mapper;
        public BranchService(IBranchRepository branchRepository, IMapper mapper, ICompanyRepository companyRepository, ICityRepository cityRepository)
        {
            _branchRepository = branchRepository;
			_companyRepository = companyRepository;
			_cityRepository = cityRepository;
			_mapper = mapper;
        }
        public async Task<BaseResponse<object>> CreateAsync(BranchCreateDto branchCreateDto)
		{
			var validator = new BranchCreateValidator();
			var validationResult = await validator.ValidateAsync(branchCreateDto);

			if (!validationResult.IsValid)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
				};
			}

			var company = await _companyRepository.GetByIdAsync(branchCreateDto.CompanyId);
			if (company is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "This company does not exist"
				};
			}
			var city = await _cityRepository.GetByIdAsync(branchCreateDto.CityId);
			if (city is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "This city does not exist"
				};
			}

			var existedBranch = await _branchRepository.GetByFilter(b => b.Name.ToLower() == branchCreateDto.Name.ToLower() && 
			b.CityId == branchCreateDto.CityId && 
			b.CompanyId == branchCreateDto.CompanyId && 
			!b.IsDeleted);

			if (existedBranch is not null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"A branch with the name '{branchCreateDto.Name}' already exists for this company in this city."
				};
			}
			var mainBranch = await _branchRepository.GetByFilter(b => b.IsMain && b.CompanyId == branchCreateDto.CompanyId);
			if (mainBranch is not null && branchCreateDto.IsMain)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Main branch already exists for this company."
				};
			}
			var branch = _mapper.Map<Branch>(branchCreateDto);
			await _branchRepository.AddAsync(branch);
			await _branchRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.Created,
				Message = "The branch is successfully created."
			};
		}

		public async Task<BaseResponse<object>> DeleteAsync(Guid id)
		{
			var branch = await _branchRepository.GetByIdAsync(id);
			if (branch is null || branch.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "The branch does not exist."
				};
			}

			branch.IsDeleted = true;
			await _branchRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The branch is successfully deleted"
			};
		}

		public async Task<BaseResponse<Pagination<BranchGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			if (pageNumber < 1 || pageSize < 1)
			{
				return new BaseResponse<Pagination<BranchGetDto>>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Page number and page size should be greater than 0."
				};
			}
			IQueryable<Branch> query = _branchRepository.GetAll(x => !x.IsDeleted);

			int totalItems = await query.CountAsync();
			if (totalItems == 0)
			{
				return new BaseResponse<Pagination<BranchGetDto>>
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

			List<BranchGetDto> branchGetDtos = await query.Select(x => new BranchGetDto { Id = x.Id, Name = x.Name, CityId = x.CityId, CompanyId = x.CompanyId, IsMain = x.IsMain, Location = x.Location }).ToListAsync();
			var pagination = new Pagination<BranchGetDto>
			{
				Items = branchGetDtos,
				TotalCount = totalItems,
				PageIndex = pageNumber,
				PageSize = isPaginated ? pageSize : totalItems,
				TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
			};

			return new BaseResponse<Pagination<BranchGetDto>>
			{
				Data = pagination,
				StatusCode = HttpStatusCode.OK,
				Message = "Branches are successfully retrieved.",
			};
		}

		public async Task<BaseResponse<BranchGetDto>> GetByIdAsync(Guid id)
		{
			var branch = await _branchRepository.GetByIdAsync(id);
			if (branch is null || branch.IsDeleted)
			{
				return new BaseResponse<BranchGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "The branch does not exist."
				};
			}
			return new BaseResponse<BranchGetDto>
			{
				Data = _mapper.Map<BranchGetDto>(branch),	
				StatusCode = HttpStatusCode.OK,
				Message = "The branch is successfully retrieved"
			};
		}

		public async Task<BaseResponse<object>> UpdateAsync(Guid id, BranchUpdateDto branchUpdateDto)
		{
			if (id != branchUpdateDto.Id)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Id does not match with the root."
				};
			}
			var validator = new BranchUpdateValidator();
			var validationResult = await validator.ValidateAsync(branchUpdateDto);

			if (!validationResult.IsValid)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(";", validationResult.Errors.Select(e => e.ErrorMessage))
				};
			}
			var branch = await _branchRepository.GetByIdAsync(id);
			if (branch is null || branch.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "The branch does not exist."
				};
			}
			var company = await _companyRepository.GetByIdAsync(branchUpdateDto.CompanyId);
			if(company is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "This company does not exist"
				};
			}
			var city = await _cityRepository.GetByIdAsync(branchUpdateDto.CityId);
			if (city is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "This city does not exist"
				};
			}
			var existingBranch = await _branchRepository.GetByFilter(b => branch.Name.ToLower() != branchUpdateDto.Name.ToLower() && 
			b.Name.ToLower() == branchUpdateDto.Name.ToLower() && 
			b.CityId == branchUpdateDto.CityId && 
			b.CompanyId == branchUpdateDto.CompanyId && 
			!b.IsDeleted);
			if (existingBranch is not null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"A branch with the name '{branchUpdateDto.Name}' already exists for this company in this city."
				};
			}
			var mainBranch = _branchRepository.GetByFilter(x => x.CompanyId == branchUpdateDto.CompanyId && x.IsMain);
			if (branchUpdateDto.IsMain && mainBranch is not null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "The main branch of this company already exists."
				};
			}
			branch.Name = branchUpdateDto.Name;
			branch.Location = branchUpdateDto.Location;
			branch.CompanyId = branchUpdateDto.CompanyId;
			branch.CityId = branchUpdateDto.CityId;
			branch.IsMain = branchUpdateDto.IsMain;
			_branchRepository.Update(branch);
			await _branchRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NoContent,
				Message = "The branch is successfully updated."
			};

		}
	}
}
