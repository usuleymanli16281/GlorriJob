using System.Net;
using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Vacancy;
using GlorriJob.Application.Validations.Vacancy;
using GlorriJob.Common.Shared;
using GlorriJob.Domain;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Shared;
using GlorriJob.Persistence.Implementations.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GlorriJob.Persistence.Implementations.Services;

public class VacancyService : IVacancyService
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICityRepository _cityRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;

    public VacancyService(IVacancyRepository vacancyRepository, ICategoryRepository categoryRepository, ICityRepository cityRepository, ICompanyRepository companyRepository, IDepartmentRepository departmentRepository, IMapper mapper)
    {
        _vacancyRepository = vacancyRepository;
        _categoryRepository = categoryRepository;
        _cityRepository = cityRepository;
        _companyRepository = companyRepository;
        _departmentRepository = departmentRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<VacancyGetDto>> GetByIdAsync(Guid id)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(id);
        if (vacancy == null || vacancy.IsDeleted)
        {
            return new BaseResponse<VacancyGetDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The vacancy does not exist."
            };
        }

        var vacancyGetDto = _mapper.Map<VacancyGetDto>(vacancy);

        return new BaseResponse<VacancyGetDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "The vacancy is successfully fetched.",
            Data = vacancyGetDto
        };
    }

    public async Task<BaseResponse<Pagination<VacancyGetDto>>> GetVacanciesAsync(VacancyFilterDto filterDto)
    {
		if (filterDto.PageNumber < 1 || filterDto.PageSize < 1)
		{
			return new BaseResponse<Pagination<VacancyGetDto>>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "Page number and page size should be greater than 0."
			};
		}
		IQueryable<Vacancy> query = _vacancyRepository.GetAll(v => !v.IsDeleted);

        if (!string.IsNullOrEmpty(filterDto.Title))
        {
            query = query.Where(v => v.Title.Contains(filterDto.Title));
        }

        if (filterDto.VacancyType.HasValue)
        {
            query = query.Where(v => v.VacancyType == filterDto.VacancyType);
        }

        if (filterDto.JobLevel.HasValue)
        {
            query = query.Where(v => v.JobLevel == filterDto.JobLevel);
        }

        if (filterDto.ExpireDateFrom.HasValue)
        {
            query = query.Where(v => v.ExpireDate >= filterDto.ExpireDateFrom);
        }

        if (filterDto.ExpireDateTo.HasValue)
        {
            query = query.Where(v => v.ExpireDate <= filterDto.ExpireDateTo);
        }

        if (filterDto.AdditionalFilters != null)
        {
            foreach (var filter in filterDto.AdditionalFilters)
            {
                var property = filter.Key;
                var value = filter.Value;

                query = query.Where(v => EF.Property<object>(v, property).Equals(value));
            }
        }

        int totalItems = await query.CountAsync();

        if (filterDto.PageNumber > 0 && filterDto.PageSize > 0)
        {
            int skip = (filterDto.PageNumber - 1) * filterDto.PageSize;
            query = query.Skip(skip).Take(filterDto.PageSize);
        }

        var vacancyGetDtos = await query
            .Select(v => new VacancyGetDto
            {
                Id = v.Id,
                Title = v.Title,
                VacancyType = v.VacancyType,
                JobLevel = v.JobLevel,
                ExpireDate = v.ExpireDate
            })
            .ToListAsync();

        var pagination = new Pagination<VacancyGetDto>
        {
            Items = vacancyGetDtos,
            TotalCount = totalItems,
            PageIndex = filterDto.PageNumber,
            PageSize = filterDto.PageSize,
            TotalPage = (int)Math.Ceiling((double)totalItems / filterDto.PageSize)
        };

        return new BaseResponse<Pagination<VacancyGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Vacancies fetched successfully.",
            Data = pagination
        };
    }

    public async Task<BaseResponse<Pagination<VacancyGetDto>>> SearchVacanciesAsync(VacancyFilterDto filterDto)
    {
        if (filterDto.PageNumber < 1 || filterDto.PageSize < 1)
        {
            return new BaseResponse<Pagination<VacancyGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0."
            };
        }

        if (!string.IsNullOrEmpty(filterDto.Title) && filterDto.Title.Length < 3)
        {
            return new BaseResponse<Pagination<VacancyGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Search term must be at least 3 characters long."
            };
        }

        IQueryable<Vacancy> query = _vacancyRepository.GetAll(v => !v.IsDeleted);

        if (!string.IsNullOrEmpty(filterDto.Title))
        {
            query = query.Where(v => v.Title.Contains(filterDto.Title));
        }

        if (filterDto.VacancyType.HasValue)
        {
            query = query.Where(v => v.VacancyType == filterDto.VacancyType);
        }

        if (filterDto.JobLevel.HasValue)
        {
            query = query.Where(v => v.JobLevel == filterDto.JobLevel);
        }

        int totalItems = await query.CountAsync();

        if (filterDto.PageNumber > 0 && filterDto.PageSize > 0)
        {
            int skip = (filterDto.PageNumber - 1) * filterDto.PageSize;
            query = query.Skip(skip).Take(filterDto.PageSize);
        }

        var vacancyGetDtos = await query
            .Select(v => new VacancyGetDto
            {
                Id = v.Id,
                Title = v.Title,
                VacancyType = v.VacancyType,
                JobLevel = v.JobLevel,
                ExpireDate = v.ExpireDate
            })
            .ToListAsync();

        var pagination = new Pagination<VacancyGetDto>
        {
            Items = vacancyGetDtos,
            TotalCount = totalItems,
            PageIndex = filterDto.PageNumber,
            PageSize = filterDto.PageSize,
            TotalPage = (int)Math.Ceiling((double)totalItems / filterDto.PageSize)
        };

        return new BaseResponse<Pagination<VacancyGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Vacancies search completed successfully.",
            Data = pagination
        };
    }

    public async Task<BaseResponse<VacancyGetDto>> CreateAsync(VacancyCreateDto createVacancyDto)
    {
        var validator = new VacancyCreateValidator();
        var validationResult = await validator.ValidateAsync(createVacancyDto);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<VacancyGetDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

        var existedVacancy = await _vacancyRepository.GetByFilter(
            expression: v => v.Title == createVacancyDto.Title && !v.IsDeleted,
            isTracking: false);

        if (existedVacancy != null)
        {
            return new BaseResponse<VacancyGetDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "The vacancy already exists."
            };
        }
        // branchId and detailId check
        var category = await _categoryRepository.GetByIdAsync(createVacancyDto.CategoryId);
        if(category is null)
        {
			return new BaseResponse<VacancyGetDto>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "This category does not exist."
			};
		}
        var city = await _cityRepository.GetByIdAsync(createVacancyDto.CityId);
        if(city is null)
        {
            return new BaseResponse<VacancyGetDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "This city does not exist."
            };
        }
        var company = await _companyRepository.GetByIdAsync(createVacancyDto.CompanyId);
        if(company is null)
        {
			return new BaseResponse<VacancyGetDto>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "This company does not exist."
			};
		}
        var department = await _departmentRepository.GetByIdAsync(createVacancyDto.DepartmentId);
        if(department is null)
        {
			return new BaseResponse<VacancyGetDto>
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = "This department does not exist."
			};
		}
        var createdVacancy = _mapper.Map<Vacancy>(createVacancyDto);
        await _vacancyRepository.AddAsync(createdVacancy);
        await _vacancyRepository.SaveChangesAsync();

        var vacancyGetDto = _mapper.Map<VacancyGetDto>(createdVacancy);

        return new BaseResponse<VacancyGetDto>
        {
            StatusCode = HttpStatusCode.Created,
            Message = "The vacancy has been successfully created.",
            Data = vacancyGetDto
        };
    }

    public async Task<BaseResponse<VacancyGetDto>> UpdateAsync(Guid id, VacancyUpdateDto vacancyUpdateDto)
    {
        if (id != vacancyUpdateDto.Id)
        {
            return new BaseResponse<VacancyGetDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Id does not match with the route parameter."
            };
        }

        var validator = new VacancyUpdateValidator();
        var validationResult = await validator.ValidateAsync(vacancyUpdateDto);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<VacancyGetDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var vacancy = await _vacancyRepository.GetByIdAsync(id);
        if (vacancy == null || vacancy.IsDeleted)
        {
            return new BaseResponse<VacancyGetDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "The vacancy does not exist.",
                Data = null
            };
        }

        var existedVacancy = await _vacancyRepository.GetByFilter(
            expression: v => v.Title.ToLower() == vacancyUpdateDto.Title.ToLower() && v.Id != id && !v.IsDeleted,
            isTracking: false);

        if (existedVacancy is not null)
        {
            return new BaseResponse<VacancyGetDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = $"A vacancy with the title '{vacancyUpdateDto.Title}' already exists.",
                Data = null
            };
        }

        vacancy.Title = vacancyUpdateDto.Title;
        vacancy.VacancyType = vacancyUpdateDto.VacancyType;
        vacancy.JobLevel = vacancyUpdateDto.JobLevel;
        vacancy.ExpireDate = vacancyUpdateDto.ExpireDate;

        await _vacancyRepository.SaveChangesAsync();

        var updatedVacancyDto = _mapper.Map<VacancyGetDto>(vacancy);

        return new BaseResponse<VacancyGetDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "The vacancy has been successfully updated.",
            Data = updatedVacancyDto
        };
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid id)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(id);
        if (vacancy == null || vacancy.IsDeleted)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The vacancy does not exist."
            };
        }

        vacancy.IsDeleted = true;
        await _vacancyRepository.SaveChangesAsync();

        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "The vacancy has been successfully deleted."
        };
    }
}




