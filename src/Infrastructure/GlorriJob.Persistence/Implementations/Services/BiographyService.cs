using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Biography;
using GlorriJob.Application.Validations.Biography;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using System.Net;


namespace GlorriJob.Persistence.Implementations.Services
{
	public class BiographyService : IBiographyService
	{
		private IBiographyRepository _biographyRepository { get; }
		private IImageKitService _imageKitService { get; }
		private ICompanyRepository _companyRepository { get; }
		private IMapper _mapper { get; }
		public BiographyService(IBiographyRepository biographyRepository, IImageKitService imageKitService, IMapper mapper, ICompanyRepository companyRepository)
		{
			_biographyRepository = biographyRepository;
			_imageKitService = imageKitService;
			_mapper = mapper;
			_companyRepository = companyRepository;
		}
		public async Task<BaseResponse<object>> CreateAsync(BiographyCreateDto biographyCreateDto)
		{
			var validation = new BiographyCreateValidator();
			var validationResult = await validation.ValidateAsync(biographyCreateDto);
			if (!validationResult.IsValid)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
				};
			}
			var biography = await _biographyRepository.GetByFilter(b =>
			b.Key.ToLower() == biographyCreateDto.Key.ToLower() &&
			!b.IsDeleted);
			if (biography is not null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"A biography with the key '{biographyCreateDto.Key}' already exists."
				};
			}
			var company = await _companyRepository.GetByIdAsync(biographyCreateDto.CompanyId);
			if (company is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "This company does not exist."
				};
			}
			var iconPath = biographyCreateDto.Icon is not null ? await UploadImageAsync(biographyCreateDto.Icon) : null;
			if (iconPath is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Error occured while uploading an icon."
				};
			}

			biography = _mapper.Map<Biography>(biographyCreateDto);
			biography.Icon = iconPath;
			await _biographyRepository.AddAsync(biography);
			await _biographyRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.Created,
				Message = "The biography is successfully created."
			};
		}

		public async Task<BaseResponse<object>> DeleteAsync(Guid id)
		{
			var biography = await _biographyRepository.GetByIdAsync(id);
			if (biography is null || biography.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The biography does not exist."
				};
			}
			biography.IsDeleted = true;
			var iconId = biography.Icon is not null ? await _imageKitService.GetImageId(biography.Icon) : null;
			var isIconDeleted = iconId is not null ? await _imageKitService.DeleteImageAsync(iconId) : true;
			if (isIconDeleted == false)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Error occured while deleting an icon"
				};
			}

			await _biographyRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NoContent,
				Message = "The biography is successfully deleted."
			};
		}

		public async Task<BaseResponse<Pagination<BiographyGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			if (pageNumber < 1 || pageSize < 1)
			{
				return new BaseResponse<Pagination<BiographyGetDto>>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Page number and page size should be greater than 0."
				};
			}
			IQueryable<Biography> query = _biographyRepository.GetAll(b => !b.IsDeleted);

			int totalItems = await query.CountAsync();
			if (totalItems == 0)
			{
				new BaseResponse<Pagination<BiographyGetDto>>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The biographies are not found"
				};
			}
			if (isPaginated)
			{
				int skip = (pageNumber - 1) * pageSize;
				query = query.Skip(skip).Take(pageSize);
			}
			List<BiographyGetDto> biographyGetDtos = await query.Select(c =>
				new BiographyGetDto
				{
					Value = c.Value,
					Key = c.Key,
					Icon = c.Icon
				}).ToListAsync();
			return new BaseResponse<Pagination<BiographyGetDto>>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The biographies are successfully retrieved.",
				Data = new Pagination<BiographyGetDto>
				{
					Items = biographyGetDtos,
					TotalCount = totalItems,
					PageIndex = pageNumber,
					PageSize = isPaginated ? pageSize : totalItems,
					TotalPage = (int)Math.Ceiling((double)totalItems / pageSize)
				}
			};
		}

		public async Task<BaseResponse<BiographyGetDto>> GetByIdAsync(Guid id)
		{
			var biography = await _biographyRepository.GetByFilter(b => b.Id == id && !b.IsDeleted);
			if (biography is null)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The biography does not exist."
				};
			}
			return new BaseResponse<BiographyGetDto>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The biography is successfully retrieved.",
				Data = _mapper.Map<BiographyGetDto>(biography)
			};
		}
		public async Task<BaseResponse<object>> UpdateAsync(Guid id, BiographyUpdateDto biographyUpdateDto)
		{
			if (biographyUpdateDto.Id != id)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Id does not match with the route parameter."
				};
			}
			var validator = new BiographyUpdateValidator();
			var validationResult = await validator.ValidateAsync(biographyUpdateDto);
			if (!validationResult.IsValid)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
				};
			}
			var biography = await _biographyRepository.GetByIdAsync(id);
			if (biography is null || biography.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The biography does not exist."
				};
			}
			var existedBiography = await _biographyRepository.GetByFilter(b =>
			biography.Key != biographyUpdateDto.Key &&
			b.Key.ToLower() == biographyUpdateDto.Key.ToLower() &&
			!b.IsDeleted);

			if (existedBiography is not null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"A biography with the key '{biographyUpdateDto.Key}' already exists."
				};
			}
			var company = await _companyRepository.GetByIdAsync(biographyUpdateDto.CompanyId);
			if (company is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "This company Id does not exist."
				};
			}
			string? newIconPath = biographyUpdateDto.Icon is not null ? await UploadImageAsync(biographyUpdateDto.Icon) : null;
			if (biographyUpdateDto.Icon is not null && newIconPath is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Error occured while uploading an icon."
				};
			}
			var previousLogoImageId = biography.Icon is not null ? await _imageKitService.GetImageId(biography.Icon) : null;
			var isPreviousLogoImageDeleted = previousLogoImageId is not null ? await _imageKitService.DeleteImageAsync(previousLogoImageId) : true;
			if (isPreviousLogoImageDeleted == false)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Error occured while deleting a previous icon"
				};
			}
			biography.Key = biographyUpdateDto.Key;
			biography.Value = biographyUpdateDto.Value;
			biography.Icon = newIconPath;
			await _biographyRepository.SaveChangesAsync();
			var biographyGetDto = _mapper.Map<BiographyGetDto>(biography);
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The biography is successfully updated"
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
