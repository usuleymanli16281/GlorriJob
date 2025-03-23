using GlorriJob.Application.Dtos.Biography;
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
	public interface IVacancyDetailService
	{
		Task<BaseResponse<VacancyDetailGetDto>> GetByIdAsync(Guid id);
		Task<BaseResponse<Pagination<VacancyDetailGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
		Task<BaseResponse<VacancyDetailGetDto>> CreateAsync(VacancyDetailCreateDto vacancyDetailCreateDto);
		Task<BaseResponse<VacancyDetailGetDto>> UpdateAsync(Guid id, VacancyDetailUpdateDto vacancyDetailUpdateDto);
		Task<BaseResponse<object>> DeleteAsync(Guid id);
	}
}
