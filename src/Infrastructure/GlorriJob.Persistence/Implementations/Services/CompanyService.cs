using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Application.Dtos.Company;
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Application.Dtos.Industry;
using GlorriJob.Application.Validations.Company;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;


namespace GlorriJob.Persistence.Implementations.Services
{
	public class CompanyService : ICompanyService
	{
		private ICompanyRepository _companyRepository { get; }
		private IIndustryService _industryService { get; }
		private IImageKitService _imageKitService { get; }
		private IMapper _mapper { get; }
        public CompanyService(ICompanyRepository companyRepository, IMapper mapper, IIndustryService industryService, IImageKitService imageKitService)
        {
            _companyRepository = companyRepository;
			_industryService = industryService;
			_mapper = mapper;
			_imageKitService = imageKitService;
        }
        public async Task<BaseResponse<CompanyGetDto>> CreateAsync(CompanyCreateDto companyCreateDto)
		{
			var validation = new CompanyCreateValidator();
			var validationResult = await validation.ValidateAsync(companyCreateDto);
			if(!validationResult.IsValid )
			{
				return new BaseResponse<CompanyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
					Data = null
				};
			}
			var existedCompany = await _companyRepository.GetByFilter(c => 
			c.Name.ToLower().Contains(companyCreateDto.Name.ToLower()) && 
			!c.IsDeleted);
			if(existedCompany is not null) 
			{
				return new BaseResponse<CompanyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"A company with the name '{companyCreateDto.Name}' already exists.",
					Data = null
				};
			}
			var industryResponse = await _industryService.GetByIdAsync(companyCreateDto.IndustryId);
			if(industryResponse.Data is null )
			{
				return new BaseResponse<CompanyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"The IndustryId does not exist.",
					Data = null
				};
			}
			string imageName = companyCreateDto.Logo.FileName;
			string tempImagePath = Path.Combine(Path.GetTempPath(), imageName);
			using (var stream = new FileStream(tempImagePath, FileMode.Create))
			{
				await companyCreateDto.Logo.CopyToAsync(stream);
			}
			var response = await _imageKitService.AddImageAsync(tempImagePath, imageName);
			File.Delete(tempImagePath);
			var createdCompany = _mapper.Map<Company>(companyCreateDto);
			await _companyRepository.AddAsync(createdCompany);
			await _companyRepository.SaveChangesAsync();
			var companyGetDto = _mapper.Map<CompanyGetDto>(createdCompany);
			return new BaseResponse<CompanyGetDto>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The company is successfully created.",
				Data = companyGetDto
			};
		}

		public async Task<BaseResponse<object>> DeleteAsync(Guid id)
		{
			var company = await _companyRepository.GetByIdAsync(id);
			if(company is null || company.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "The company does not exist.",
					Data = null
				};
			}
			company.IsDeleted = true;
			await _companyRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The company is successfully deleted.",
				Data = null
			};
		}

		public async Task<BaseResponse<Pagination<CompanyGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			if(pageNumber < 1 || pageSize < 1)
			{
				return new BaseResponse<Pagination<CompanyGetDto>>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Page number and page size should be greater than 0.",
					Data = null
				};
			}
			IQueryable<Company> query = _companyRepository.GetAll(
				expression: c => !c.IsDeleted , 
				includes: new[] { "Industry", "Departments" });

			int totalItems = await query.CountAsync();
			if (isPaginated)
			{
				int skip = (pageNumber - 1) * pageSize;
				query = query.Skip(skip).Take(pageSize);
			}
			List<CompanyGetDto> companyGetDtos = await query.Select(c =>
				new CompanyGetDto
				{
					Id = c.Id,
					Name = c.Name,
					EmployeeCount = c.EmployeeCount,
					FoundedYear = c.FoundedYear,
					LogoPath = c.LogoPath,
					PosterPath = c.PosterPath,
					ExistedVacancy = c.ExistedVacancy,
					IndustryGetDto = new IndustryGetDto
					{
						Id = c.Industry.Id,
						Name = c.Industry.Name
					},
					DepartmentGetDtos = c.Departments.Select(d => new DepartmentGetDto
					{
						Id = d.Id,
						Name = d.Name,
					}).ToList()
				}).ToListAsync();
			return new BaseResponse<Pagination<CompanyGetDto>>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The companies are successfully retrieved.",
				Data = new Pagination<CompanyGetDto>
				{
					Items = companyGetDtos,
					TotalCount = totalItems,
					PageIndex = pageNumber,
					PageSize = isPaginated ? pageSize : totalItems,
					TotalPage = (int)Math.Ceiling((double)totalItems / pageSize)
				}
			};
		}

		public async Task<BaseResponse<CompanyGetDto>> GetByIdAsync(Guid id)
		{
			var company = await _companyRepository.GetByFilter(
				expression: c => c.Id == id && !c.IsDeleted,
				includes: new[] {"Industry","Departments"});
			if (company is null)
			{
				return new BaseResponse<CompanyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "The company does not exist.",
					Data = null
				};
			}
			return new BaseResponse<CompanyGetDto>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The company is successfully retrieved.",
				Data = _mapper.Map<CompanyGetDto>(company)
			};
		}

		public async Task<BaseResponse<Pagination<CompanyGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			if (pageNumber < 1 || pageSize < 1)
			{
				return new BaseResponse<Pagination<CompanyGetDto>>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Page number and page size should be greater than 0.",
					Data = null
				};
			}
			IQueryable<Company> query = _companyRepository.GetAll(
				expression: c => !c.IsDeleted &&
				c.Name.ToLower().Contains(name.ToLower()), 
				includes: new[] { "Industry", "Departments" });

			int totalItems = await query.CountAsync();
			if (isPaginated)
			{
				int skip = (pageNumber - 1) * pageSize;
				query = query.Skip(skip).Take(pageSize);
			}
			List<CompanyGetDto> companyGetDtos = await query.Select(c =>
				new CompanyGetDto
				{
					Id = c.Id,
					Name = c.Name,
					EmployeeCount = c.EmployeeCount,
					FoundedYear = c.FoundedYear,
					LogoPath = c.LogoPath,
					PosterPath = c.PosterPath,
					ExistedVacancy = c.ExistedVacancy,
					IndustryGetDto = new IndustryGetDto
					{
						Id = c.Industry.Id,
						Name = c.Industry.Name
					},
					DepartmentGetDtos = c.Departments.Select(d => new DepartmentGetDto
					{
						Id = d.Id,
						Name = d.Name,
					}).ToList()
				}).ToListAsync();
			return new BaseResponse<Pagination<CompanyGetDto>>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The companies are successfully retrieved.",
				Data = new Pagination<CompanyGetDto>
				{
					Items = companyGetDtos,
					TotalCount = totalItems,
					PageIndex = pageNumber,
					PageSize = isPaginated ? pageSize : totalItems,
					TotalPage = (int)Math.Ceiling((double)totalItems / pageSize)
				}
			};
		}

		public async Task<BaseResponse<CompanyGetDto>> UpdateAsync(Guid id, CompanyUpdateDto companyUpdateDto)
		{
			if(companyUpdateDto.Id != id)
			{
				return new BaseResponse<CompanyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Id does not match with the route parameter.",
					Data = null
				};
			}
			var validator = new CompanyUpdateValidator();
			var validationResult = await validator.ValidateAsync(companyUpdateDto);
			if (!validationResult.IsValid)
			{
				return new BaseResponse<CompanyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
					Data = null
				};
			}
			var company = await _companyRepository.GetByIdAsync(id);
			if (company is null || company.IsDeleted)
			{
				return new BaseResponse<CompanyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "The company does not exist.",
					Data = null
				};
			}
			var existedCompany = await _companyRepository.GetByFilter(c =>
			c.Name.ToLower().Contains(companyUpdateDto.Name.ToLower()) &&
			!c.IsDeleted);
			
			if(existedCompany is not null)
			{
				return new BaseResponse<CompanyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"A company with the name '{companyUpdateDto.Name}' already exists.",
					Data = null
				};
			}
			var industryResponse = await _industryService.GetByIdAsync(companyUpdateDto.IndustryId);
			if (industryResponse.Data is null)
			{
				return new BaseResponse<CompanyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"The IndustryId does not exist.",
					Data = null
				};
			}
			company.Name = companyUpdateDto.Name;
			company.EmployeeCount = companyUpdateDto.EmployeeCount;
			company.FoundedYear = companyUpdateDto.FoundedYear;
			company.LogoPath = companyUpdateDto.LogoPath;
			company.PosterPath = companyUpdateDto.PosterPath;
			company.IndustryId = companyUpdateDto.IndustryId;

			await _companyRepository.SaveChangesAsync();	
			var companyGetDto = _mapper.Map<CompanyGetDto>(company);
			return new BaseResponse<CompanyGetDto>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The company is successfully updated",
				Data = companyGetDto
			};
		}
	}
}
