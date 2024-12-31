using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Company;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Implementations.Services
{
	public class CompanyService : ICompanyService
	{
		public Task<BaseResponse<CompanyGetDto>> CreateAsync(CompanyCreateDto companyCreateDto)
		{
			throw new NotImplementedException();
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
