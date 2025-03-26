using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.CompanyDetail;
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Application.Validations.CompanyDetail;
using GlorriJob.Application.Validations.VacancyDetail;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using GlorriJob.Persistence.Implementations.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Implementations.Services
{
	public class CompanyDetailService : ICompanyDetailService
	{
		private ICompanyDetailRepository _companyDetailRepository { get; }
		private ICompanyRepository _companyRepository { get; }
		private IMapper _mapper {  get; }
        public CompanyDetailService(ICompanyDetailRepository companyDetailRepository, ICompanyRepository companyRepository, IMapper mapper)
        {
			_companyDetailRepository = companyDetailRepository;
			_companyRepository = companyRepository;
			_mapper = mapper;	
        }
        public async Task<BaseResponse<object>> CreateAsync(CompanyDetailCreateDto companyDetailCreateDto)
		{
			var validation = new CompanyDetailCreateValidator();
			var validationResult = await validation.ValidateAsync(companyDetailCreateDto);
			if (!validationResult.IsValid)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
				};
			}
			var company = await _companyRepository.GetByIdAsync(companyDetailCreateDto.CompanyId);
			if (company is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "This company does not exist."
				};
			}

			var vacancyDetail = _mapper.Map<CompanyDetail>(companyDetailCreateDto);
			await _companyDetailRepository.AddAsync(vacancyDetail);
			await _companyDetailRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.Created,
				Message = "The company detail is successfully created."
			};
		}

		public async Task<BaseResponse<object>> DeleteAsync(Guid id)
		{
			var companyDetail = await _companyDetailRepository.GetByIdAsync(id);
			if (companyDetail is null || companyDetail.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The company detail does not exist."
				};
			}
			companyDetail.IsDeleted = true;


			await _companyDetailRepository.SaveChangesAsync();
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NoContent,
				Message = "The company detail is successfully deleted."
			};
		}

		public async Task<BaseResponse<CompanyDetailGetDto>> GetByCompanyIdAsync(Guid companyId)
		{
			var company = await _companyRepository.GetByIdAsync(companyId);
			if(company is null || company.IsDeleted)
			{
				return new BaseResponse<CompanyDetailGetDto>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "This company does not exist."
				};
			}
			var companyDetail =  await _companyDetailRepository.GetByFilter(x => x.CompanyId == companyId);
			if (companyDetail is null || companyDetail.IsDeleted)
			{
				return new BaseResponse<CompanyDetailGetDto>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The company detail does not exist."
				};
			}
			return new BaseResponse<CompanyDetailGetDto>
			{
				Data = _mapper.Map<CompanyDetailGetDto>(companyDetail),
				StatusCode = HttpStatusCode.OK,
				Message = "The company detail is successfully retrieved."
			};
		}

		public async Task<BaseResponse<CompanyDetailGetDto>> GetByIdAsync(Guid id)
		{
			var companyDetail = await _companyDetailRepository.GetByIdAsync(id);
			if (companyDetail is null || companyDetail.IsDeleted)
			{
				return new BaseResponse<CompanyDetailGetDto>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The company detail does not exist."
				};
			}
			return new BaseResponse<CompanyDetailGetDto>
			{
				Data = _mapper.Map<CompanyDetailGetDto>(companyDetail),
				StatusCode = HttpStatusCode.OK,
				Message = "The company detail is successfully retrieved."
			};
		}

		public async Task<BaseResponse<object>> UpdateAsync(Guid id, CompanyDetailUpdateDto companyDetailUpdateDto)
		{
			if (companyDetailUpdateDto.Id != id)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Id does not match with the route parameter."
				};
			}
			var validator = new CompanyDetailUpdateValidator();
			var validationResult = await validator.ValidateAsync(companyDetailUpdateDto);
			if (!validationResult.IsValid)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
				};
			}
			var companyDetail = await _companyDetailRepository.GetByIdAsync(id);
			if (companyDetail is null || companyDetail.IsDeleted)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "The company detail does not exist."
				};
			}
			var company = await _companyRepository.GetByIdAsync(companyDetailUpdateDto.CompanyId);
			if (company is null)
			{
				return new BaseResponse<object>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "This company does not exist."
				};
			}
			companyDetail.CompanyId = companyDetailUpdateDto.CompanyId;
			companyDetail.Content = companyDetailUpdateDto.Content;

			_companyDetailRepository.Update(companyDetail);
			await _companyDetailRepository.SaveChangesAsync();

			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "The company detail is successfully updated.",
			};
		}
	}
}
