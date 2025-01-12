﻿using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Biography;
using GlorriJob.Application.Dtos.Company;
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Application.Dtos.Industry;
using GlorriJob.Application.Validations.Biography;
using GlorriJob.Application.Validations.Company;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Implementations.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GlorriJob.Persistence.Implementations.Services
{
	public class BiographyService : IBiographyService
	{
		private IBiographyRepository _biographyRepository { get; }
		private IImageKitService _imageKitService { get; }
		private ICompanyService _companyService { get; }
		private IMapper _mapper { get; }
		public BiographyService(IBiographyRepository biographyRepository, IImageKitService imageKitService, IMapper mapper, ICompanyService companyService)
		{
			_biographyRepository = biographyRepository;
			_imageKitService = imageKitService;
			_mapper = mapper;
			_companyService = companyService;
		}
		public async Task<BaseResponse<BiographyGetDto>> CreateAsync(BiographyCreateDto biographyCreateDto)
		{
			var validation = new BiographyCreateValidator();
			var validationResult = await validation.ValidateAsync(biographyCreateDto);
			if (!validationResult.IsValid)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
					Data = null
				};
			}
			var existedBiography = await _biographyRepository.GetByFilter(b =>
			b.Key.ToLower() == biographyCreateDto.Key.ToLower() &&
			!b.IsDeleted);
			if (existedBiography is not null)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"A biography with the key '{biographyCreateDto.Key}' already exists.",
					Data = null
				};
			}
			var companyGetDto = (await _companyService.GetByIdAsync(biographyCreateDto.CompanyId)).Data;
			if(companyGetDto is null)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"This company Id does not exist.",
					Data = null
				};
			}
			var iconPath = biographyCreateDto.Icon is not null ? await UploadImageAsync(biographyCreateDto.Icon) : null;
			if (iconPath is null)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Error occured while uploading an icon.",
					Data = null
				};
			}

			var createdBiography = _mapper.Map<Biography>(biographyCreateDto);
			createdBiography.Icon = iconPath;
			await _biographyRepository.AddAsync(createdBiography);
			await _biographyRepository.SaveChangesAsync();
			var biographyGetDto = _mapper.Map<BiographyGetDto>(createdBiography);
			return new BaseResponse<BiographyGetDto>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The biography is successfully created.",
				Data = biographyGetDto
			};
		}

		public async Task<BaseResponse<object>> DeleteAsync(Guid id)
		{
			var biography = await _biographyRepository.GetByIdAsync(id);
			if (biography is null || biography.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "The biography does not exist.",
					Data = null
				};
			}
			biography.IsDeleted = true;
			var iconId = biography.Icon is not null ? await _imageKitService.GetImageId(biography.Icon) : null;
			if (biography.Icon is not null && iconId is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Icon does not exist",
					Data = null
				};
			}
			var isIconDeleted = iconId is not null ? await _imageKitService.DeleteImageAsync(iconId) : true;
			if (isIconDeleted == false)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Error occured while deleting an icon",
					Data = null
				};
			}

			await _biographyRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The biography is successfully deleted.",
				Data = null
			};
		}

		public async Task<BaseResponse<Pagination<BiographyGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			if (pageNumber < 1 || pageSize < 1)
			{
				return new BaseResponse<Pagination<BiographyGetDto>>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Page number and page size should be greater than 0.",
					Data = null
				};
			}
			IQueryable<Biography> query = _biographyRepository.GetAll(b => !b.IsDeleted);

			int totalItems = await query.CountAsync();
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
					StatusCode = HttpStatusCode.BadRequest,
					Message = "The biography does not exist.",
					Data = null
				};
			}
			return new BaseResponse<BiographyGetDto>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The biography is successfully retrieved.",
				Data = _mapper.Map<BiographyGetDto>(biography)
			};
		}
		public async Task<BaseResponse<BiographyGetDto>> UpdateAsync(Guid id, BiographyUpdateDto biographyUpdateDto)
		{
			if (biographyUpdateDto.Id != id)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Id does not match with the route parameter.",
					Data = null
				};
			}
			var validator = new BiographyUpdateValidator();
			var validationResult = await validator.ValidateAsync(biographyUpdateDto);
			if (!validationResult.IsValid)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
					Data = null
				};
			}
			var biography = await _biographyRepository.GetByIdAsync(id);
			if (biography is null || biography.IsDeleted)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "The biography does not exist.",
					Data = null
				};
			}
			var existedBiography = await _biographyRepository.GetByFilter(b =>
			biography.Key != biographyUpdateDto.Key &&
			b.Key.ToLower() == biographyUpdateDto.Key.ToLower() &&
			!b.IsDeleted);

			if (existedBiography is not null)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"A biography with the key '{biographyUpdateDto.Key}' already exists.",
					Data = null
				};
			}
			var companyGetDto = (await _companyService.GetByIdAsync(biographyUpdateDto.CompanyId)).Data;
			if (companyGetDto is null)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"This company Id does not exist.",
					Data = null
				};
			}
			string? newIconPath = biographyUpdateDto.Icon is not null ? await UploadImageAsync(biographyUpdateDto.Icon) : null;
			if (biographyUpdateDto.Icon is not null && newIconPath is null)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Error occured while uploading an icon.",
					Data = null
				};
			}
			var previousLogoImageId = biography.Icon is not null ? await _imageKitService.GetImageId(biography.Icon) : null;
			if (biography.Icon is not null && previousLogoImageId is null)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Error occured while deleting a previous icon.",
					Data = null
				};
			}
			var isPreviousLogoImageDeleted = previousLogoImageId is not null ? await _imageKitService.DeleteImageAsync(previousLogoImageId) : true;
			if (isPreviousLogoImageDeleted == false)
			{
				return new BaseResponse<BiographyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Error occured while deleting a previous icon",
					Data = null
				};
			}
			biography.Key = biographyUpdateDto.Key;
			biography.Value = biographyUpdateDto.Value;
			biography.Icon = newIconPath;
			await _biographyRepository.SaveChangesAsync();
			var biographyGetDto = _mapper.Map<BiographyGetDto>(biography);
			return new BaseResponse<BiographyGetDto>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The biography is successfully updated",
				Data = biographyGetDto
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
