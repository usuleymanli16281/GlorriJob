using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Company;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Abstractions.Services
{
	public interface ICompanyService
	{
		Task<BaseResponse<CompanyGetDto>> GetByIdAsync(Guid id);
		Task<BaseResponse<Pagination<CompanyGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
		Task<BaseResponse<Pagination<CompanyGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
		Task<BaseResponse<CompanyGetDto>> CreateAsync(CompanyCreateDto companyCreateDto);
		Task<BaseResponse<CompanyGetDto>> UpdateAsync(Guid id, CompanyUpdateDto companyUpdateDto);
		Task<BaseResponse<object>> DeleteAsync(Guid id);
	}
}
