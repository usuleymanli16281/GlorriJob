using GlorriJob.Application.Dtos.Biography;
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
	public interface IBiographyService
	{
		Task<BaseResponse<BiographyGetDto>> GetByIdAsync(Guid id);
		Task<BaseResponse<Pagination<BiographyGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
		Task<BaseResponse<Pagination<BiographyGetDto>>> SearchByKeyAsync(string key, int pageNumber = 1, int pageSize = 10, bool isPaginated = false);
		Task<BaseResponse<BiographyGetDto>> CreateAsync(BiographyCreateDto biographyCreateDto);
		Task<BaseResponse<BiographyGetDto>> UpdateAsync(Guid id, BiographyUpdateDto biographyUpdateDto);
		Task<BaseResponse<object>> DeleteAsync(Guid id);
	}
}
