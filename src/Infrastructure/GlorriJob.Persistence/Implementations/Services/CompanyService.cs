using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Application.Dtos.Company;
using GlorriJob.Application.Validations.Company;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Implementations.Services
{
	public class CompanyService : ICompanyService
	{
		private ICompanyRepository _companyRepository { get; }
        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
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
			var existedCompany = _companyRepository.GetByFilter(
				expression: c => c.Name.ToLower().Contains(companyCreateDto.Name.ToLower()) && !c.IsDeleted,
				isTracking: false);
			if(existedCompany is not null) 
			{
				return new BaseResponse<CompanyGetDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"A company with the name '{companyCreateDto.Name}' already exists.",
					Data = null
				};
			}
			return new BaseResponse<CompanyGetDto>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "",
				Data = null
			};
		}

		public Task<BaseResponse<object>> DeleteAsync(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task<BaseResponse<Pagination<CompanyGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			throw new NotImplementedException();
		}

		public Task<BaseResponse<CompanyGetDto>> GetByIdAsync(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task<BaseResponse<Pagination<CompanyGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
		{
			throw new NotImplementedException();
		}

		public Task<BaseResponse<CompanyGetDto>> UpdateAsync(Guid id, CompanyUpdateDto companyUpdateDto)
		{
			throw new NotImplementedException();
		}
	}
}
