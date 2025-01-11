using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Biography;
using GlorriJob.Application.Dtos.Company;
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Application.Dtos.Industry;
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

namespace GlorriJob.Persistence.Implementations.Services
{
	public class BiographyService : IBiographyService
	{
		private IBiographyRepository _biographyRepository { get; }
		private IImageKitService _imageKitService { get; }
		private IMapper _mapper { get; }
		public BiographyService(IBiographyRepository biographyRepository, IImageKitService imageKitService, IMapper mapper)
		{
			_biographyRepository = biographyRepository;
			_imageKitService = imageKitService;
			_mapper = mapper;
		}
		public async Task<BaseResponse<BiographyGetDto>> CreateAsync(BiographyCreateDto biographyCreateDto)
		{
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
			if (biography.Icon is not null)
			{
				var iconId = await _imageKitService.GetImageId(biography.Icon);
				if (iconId is null)
				{
					return new BaseResponse<object>
					{
						StatusCode = HttpStatusCode.BadRequest,
						Message = $"Icon does not exist",
						Data = null
					};
				}
				var isIconDeleted = await _imageKitService.DeleteImageAsync(iconId);
				if (isIconDeleted == false)
				{
					return new BaseResponse<object>
					{
						StatusCode = HttpStatusCode.BadRequest,
						Message = $"Error occured while deleting an icon",
						Data = null
					};
				}
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
			IQueryable<Biography> query = _biographyRepository.GetAll(
				expression: b => !b.IsDeleted);

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
				Message = "The companies are successfully retrieved.",
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
			var biography = await _biographyRepository.GetByFilter(
				expression: b => b.Id == id && !b.IsDeleted);
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

		public Task<BaseResponse<Pagination<BiographyGetDto>>> SearchByKeyAsync(string key, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			throw new NotImplementedException();
		}

		public async Task<BaseResponse<BiographyGetDto>> UpdateAsync(Guid id, BiographyUpdateDto biographyUpdateDto)
		{
			throw new NotImplementedException();
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
