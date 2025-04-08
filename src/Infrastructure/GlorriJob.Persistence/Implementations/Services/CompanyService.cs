using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Company;
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Application.Dtos.Industry;
using GlorriJob.Application.Validations.Company;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;


namespace GlorriJob.Persistence.Implementations.Services
{
	public class CompanyService : ICompanyService
	{
		private ICompanyRepository _companyRepository { get; }
		private IIndustryRepository _industryRepository { get; }
		private IImageKitService _imageKitService { get; }
		private IMapper _mapper { get; }
		public CompanyService(ICompanyRepository companyRepository, IMapper mapper, IIndustryRepository industryRepository, IImageKitService imageKitService)
		{
			_companyRepository = companyRepository;
			_industryRepository = industryRepository;
			_imageKitService = imageKitService;
			_mapper = mapper;
		}
		public async Task<BaseResponse<object>> CreateAsync(CompanyCreateDto companyCreateDto)
		{
			var validation = new CompanyCreateValidator();
			var validationResult = await validation.ValidateAsync(companyCreateDto);
			if (!validationResult.IsValid)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
				};
			}
			var existedCompany = await _companyRepository.GetByFilter(c =>
			c.Name.ToLower() == companyCreateDto.Name.ToLower() &&
			!c.IsDeleted);
			if (existedCompany is not null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"A company with the name '{companyCreateDto.Name}' already exists."
				};
			}
			var industry = await _industryRepository.GetByIdAsync(companyCreateDto.IndustryId);
			if (industry is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"The industry does not exist."
				};
			}
			var logoPath = await UploadImageAsync(companyCreateDto.Logo);
			if (logoPath is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Error occured while uploading a logo image.",
					Data = null
				};
			}
			string? posterPath = null;
			if (companyCreateDto.Poster is not null)
			{
				posterPath = await UploadImageAsync(companyCreateDto.Poster);
				if (posterPath is null)
				{
					return new BaseResponse<object>
					{
						StatusCode = HttpStatusCode.BadRequest,
						Message = $"Error occured while uploading a poster image."
					};
				}
			}
			var createdCompany = _mapper.Map<Company>(companyCreateDto);
			createdCompany.LogoPath = logoPath;
			createdCompany.PosterPath = posterPath;
			await _companyRepository.AddAsync(createdCompany);
			await _companyRepository.SaveChangesAsync();
			var companyGetDto = _mapper.Map<CompanyGetDto>(createdCompany);
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.Created,
				Message = "The company is successfully created."
			};

		}

		public async Task<BaseResponse<object>> DeleteAsync(Guid id)
		{
			var company = await _companyRepository.GetByIdAsync(id);
			if (company is null || company.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The company does not exist."
				};
			}
			company.IsDeleted = true;
			var logoImageId = await _imageKitService.GetImageId(company.LogoPath);
			var isLogoImageDeleted = logoImageId is not null ? await _imageKitService.DeleteImageAsync(logoImageId) : true;
			if (isLogoImageDeleted == false)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Error occured while deleting a logo image"
				};
			}
			if (company.PosterPath is not null)
			{
				var posterImageId = await _imageKitService.GetImageId(company.PosterPath);
				var isPosterImageDeleted = posterImageId is not null ? await _imageKitService.DeleteImageAsync(posterImageId) : true;
				if (isPosterImageDeleted == false)
				{
					return new BaseResponse<object>
					{
						StatusCode = HttpStatusCode.BadRequest,
						Message = $"Error occured while deleting a poster image"
					};
				}
			}
			await _companyRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NoContent,
				Message = "The company is successfully deleted."
			};
		}

		public async Task<BaseResponse<Pagination<CompanyGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
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
				expression: c => !c.IsDeleted,
				includes: ["Industry", "Departments"]);

			int totalItems = await query.CountAsync();
			if (totalItems == 0)
			{
				return new BaseResponse<Pagination<CompanyGetDto>>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The company does not exist"
				};
			}
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
					IndustryName = c.Industry.Name,
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
				includes: ["Industry", "Departments"]);
			if (company is null)
			{
				return new BaseResponse<CompanyGetDto>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The company does not exist."
				};
			}
			var companyGetDto = new CompanyGetDto
			{
				Id = company.Id,
				Name = company.Name,
				EmployeeCount = company.EmployeeCount,
				FoundedYear = company.FoundedYear,
				LogoPath = company.LogoPath,
				PosterPath = company.PosterPath,
				ExistedVacancy = company.ExistedVacancy,
				IndustryName = company.Industry.Name,
			};
			return new BaseResponse<CompanyGetDto>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The company is successfully retrieved.",
				Data = companyGetDto
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
			if (totalItems == 0)
			{
				return new BaseResponse<Pagination<CompanyGetDto>>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The company does not exist"
				};
			}
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
					IndustryName = c.Industry.Name,
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

		public async Task<BaseResponse<object>> UpdateAsync(Guid id, CompanyUpdateDto companyUpdateDto)
		{
			if (companyUpdateDto.Id != id)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Id does not match with the route parameter."
				};
			}
			var validator = new CompanyUpdateValidator();
			var validationResult = await validator.ValidateAsync(companyUpdateDto);
			if (!validationResult.IsValid)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
				};
			}
			var company = await _companyRepository.GetByIdAsync(id);
			if (company is null || company.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The company does not exist."
				};
			}
			var existedCompany = await _companyRepository.GetByFilter(c =>
			(company.Name.ToLower() != companyUpdateDto.Name.ToLower() && c.Name.ToLower() == companyUpdateDto.Name.ToLower()) &&
			!c.IsDeleted);

			if (existedCompany is not null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"A company with the name '{companyUpdateDto.Name}' already exists."
				};
			}
			var industry = await _industryRepository.GetByIdAsync(companyUpdateDto.IndustryId);
			if (industry is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "The industry does not exist."
				};
			}
			string? newLogoPath = await UploadImageAsync(companyUpdateDto.Logo);
			if (newLogoPath is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Error occured while uploading a logo image."
				};
			}
			var previousLogoImageId = await _imageKitService.GetImageId(company.LogoPath);
			var isPreviousLogoImageDeleted = previousLogoImageId is not null ? await _imageKitService.DeleteImageAsync(previousLogoImageId) : true;
			if (isPreviousLogoImageDeleted == false)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Error occured while deleting a previous logo image"
				};
			}
			string? newPosterPath = company.PosterPath;
			if (companyUpdateDto.Poster is not null)
			{
				newPosterPath = await UploadImageAsync(companyUpdateDto.Poster);
				if (newPosterPath is null)
				{
					return new BaseResponse<object>
					{
						StatusCode = HttpStatusCode.BadRequest,
						Message = $"Error occured while uploading a poster image."
					};
				};
				if (company.PosterPath is not null)
				{

					var previousPosterImageId = await _imageKitService.GetImageId(company.PosterPath);
					var isPreviousPosterImageDeleted = previousPosterImageId is not null ? await _imageKitService.DeleteImageAsync(previousPosterImageId) : true;
					if (isPreviousPosterImageDeleted == false)
					{
						return new BaseResponse<object>
						{
							StatusCode = HttpStatusCode.BadRequest,
							Message = $"Error occured while deleting a previous logo image"
						};
					}
				}
			}
			company.Name = companyUpdateDto.Name;
			company.EmployeeCount = companyUpdateDto.EmployeeCount;
			company.FoundedYear = companyUpdateDto.FoundedYear;
			company.LogoPath = newLogoPath;
			company.PosterPath = newPosterPath;
			company.IndustryId = companyUpdateDto.IndustryId;
			_companyRepository.Update(company);
			await _companyRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NoContent,
				Message = "The company is successfully updated"
			};
		}

		private async Task<string?> UploadImageAsync(IFormFile file)
		{
			string tempImagePath = Path.Combine(Path.GetTempPath(), file.FileName);
			using (var stream = new FileStream(tempImagePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
			var imagePath = await _imageKitService.AddImageAsync(tempImagePath, file.FileName);
			return imagePath;
		}
	}
}
