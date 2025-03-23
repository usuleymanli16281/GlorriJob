using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Biography;
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Application.Validations.Biography;
using GlorriJob.Application.Validations.VacancyDetail;
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
	public class VacancyDetailService : IVacancyDetailService
	{
		private IVacancyDetailRepository _vacancyDetailRepository {  get; }
		private IVacancyRepository _vacancyRepository { get; }
		private IMapper _mapper { get; }
        public VacancyDetailService(IVacancyDetailRepository vacancyDetailRepository, IVacancyRepository vacancyRepository, IMapper mapper)
        {
            _vacancyDetailRepository = vacancyDetailRepository;
			_vacancyRepository = vacancyRepository;
			_mapper = mapper;
        }
        public async Task<BaseResponse<VacancyDetailGetDto>> CreateAsync(VacancyDetailCreateDto vacancyDetailCreateDto)
		{
			var validation = new VacancyDetailCreateValidator();
			var validationResult = await validation.ValidateAsync(vacancyDetailCreateDto);
			if (!validationResult.IsValid)
			{
				return new BaseResponse<VacancyDetailGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
				};
			}
			var vacancy = await _vacancyRepository.GetByIdAsync(vacancyDetailCreateDto.VacancyId);
			if (vacancy is null)
			{
				return new BaseResponse<VacancyDetailGetDto>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "This vacancy does not exist."
				};
			}

			var vacancyDetail = _mapper.Map<VacancyDetail>(vacancyDetailCreateDto);
			await _vacancyDetailRepository.AddAsync(vacancyDetail);
			await _vacancyDetailRepository.SaveChangesAsync();
			var vacancyDetailGetDto = _mapper.Map<VacancyDetailGetDto>(vacancyDetailCreateDto);
			return new BaseResponse<VacancyDetailGetDto>
			{
				StatusCode = HttpStatusCode.Created,
				Message = "The vacancy detail is successfully created.",
				Data = vacancyDetailGetDto
			};
		}

		public async Task<BaseResponse<object>> DeleteAsync(Guid id)
		{
			var vacancyDetail = await _vacancyDetailRepository.GetByIdAsync(id);
			if (vacancyDetail is null || vacancyDetail.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The vacancy detail does not exist."
				};
			}
			vacancyDetail.IsDeleted = true;
			

			await _vacancyDetailRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NoContent,
				Message = "The vacancy detail is successfully deleted."
			};
		}

		public async Task<BaseResponse<Pagination<VacancyDetailGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			if (pageNumber < 1 || pageSize < 1)
			{
				return new BaseResponse<Pagination<VacancyDetailGetDto>>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Page number and page size should be greater than 0."
				};
			}
			IQueryable<VacancyDetail> query = _vacancyDetailRepository.GetAll(b => !b.IsDeleted);

			int totalItems = await query.CountAsync();
			if (totalItems == 0)
			{
				new BaseResponse<Pagination<VacancyDetailGetDto>>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The vacancy details are not found"
				};
			}
			if (isPaginated)
			{
				int skip = (pageNumber - 1) * pageSize;
				query = query.Skip(skip).Take(pageSize);
			}
			List<VacancyDetailGetDto> vacancyDetailGetDtos = await query.Select(v =>
				new VacancyDetailGetDto
				{
					Id = v.Id,
					Content = v.Content,
					Salary = v.Salary,
					VacancyId = v.Id,
					VacancyType = v.VacancyType
				}).ToListAsync();
			return new BaseResponse<Pagination<VacancyDetailGetDto>>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The vacancy details are successfully retrieved.",
				Data = new Pagination<VacancyDetailGetDto>
				{
					Items = vacancyDetailGetDtos,
					TotalCount = totalItems,
					PageIndex = pageNumber,
					PageSize = isPaginated ? pageSize : totalItems,
					TotalPage = (int)Math.Ceiling((double)totalItems / pageSize)
				}
			};
		}

		public async Task<BaseResponse<VacancyDetailGetDto>> GetByIdAsync(Guid id)
		{
			var vacancyDetail = await _vacancyDetailRepository.GetByIdAsync(id);
			if (vacancyDetail is null || vacancyDetail.IsDeleted)
			{
				return new BaseResponse<VacancyDetailGetDto>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The vacancy detail does not exist."
				};
			}
			var vacancyDetailGetDto = _mapper.Map<VacancyDetailGetDto>(vacancyDetail);
			return new BaseResponse<VacancyDetailGetDto>
			{
				StatusCode = HttpStatusCode.NoContent,
				Message = "The vacancy detail is successfully retrieved.",
				Data = vacancyDetailGetDto
			};
		}

		public async Task<BaseResponse<VacancyDetailGetDto>> UpdateAsync(Guid id, VacancyDetailUpdateDto vacancyDetailUpdateDto)
		{
			if (vacancyDetailUpdateDto.Id != id)
			{
				return new BaseResponse<VacancyDetailGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Id does not match with the route parameter."
				};
			}
			var validator = new VacancyDetailUpdateValidator();
			var validationResult = await validator.ValidateAsync(vacancyDetailUpdateDto);
			if (!validationResult.IsValid)
			{
				return new BaseResponse<VacancyDetailGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
				};
			}
			var vacancyDetail = await _vacancyDetailRepository.GetByIdAsync(id);
			if (vacancyDetail is null || vacancyDetail.IsDeleted)
			{
				return new BaseResponse<VacancyDetailGetDto>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The vacancy detail does not exist."
				};
			}
			var vacancy = await _vacancyRepository.GetByIdAsync(vacancyDetailUpdateDto.VacancyId);
			if (vacancy is null)
			{
				return new BaseResponse<VacancyDetailGetDto>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "This vacancy does not exist."
				};
			}
			vacancyDetail.VacancyType = vacancyDetailUpdateDto.VacancyType;
			vacancyDetail.VacancyId = vacancyDetailUpdateDto.VacancyId;
			vacancyDetail.Salary = vacancyDetailUpdateDto.Salary;
			vacancyDetail.Content = vacancyDetailUpdateDto.Content;

			_vacancyDetailRepository.Update(vacancyDetail);
			await _vacancyDetailRepository.SaveChangesAsync();

			return new BaseResponse<VacancyDetailGetDto>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The vacancy detail is successfully updated.",
				Data = _mapper.Map<VacancyDetailGetDto>(vacancyDetail)
			};
		}
	}
}
