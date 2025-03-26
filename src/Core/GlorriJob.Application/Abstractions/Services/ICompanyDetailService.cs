using GlorriJob.Application.Dtos.CompanyDetail;
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Abstractions.Services
{
	public interface ICompanyDetailService
	{
		Task<BaseResponse<CompanyDetailGetDto>> GetByIdAsync(Guid id);
		Task<BaseResponse<CompanyDetailGetDto>> GetByCompanyIdAsync(Guid companyId);
		Task<BaseResponse<object>> CreateAsync(CompanyDetailCreateDto companyDetailCreateDto);
		Task<BaseResponse<object>> UpdateAsync(Guid id, CompanyDetailUpdateDto companyDetailUpdateDto);
		Task<BaseResponse<object>> DeleteAsync(Guid id);
	}
}
