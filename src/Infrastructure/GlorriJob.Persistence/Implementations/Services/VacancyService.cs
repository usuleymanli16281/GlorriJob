using System.Net;
using AutoMapper;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Dtos.Company;
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
    private IVacancyRepository _vacancyRepository { get; }
    private ICategoryRepository _categoryRepository { get; }
    private ICompanyRepository _companyRepository { get; }
    private IDepartmentRepository _departmentRepository { get; }
    private ICityRepository _cityRepository { get; }
    private IBranchRepository _branchRepository { get; }
    private IMapper _mapper { get; }

    public VacancyService(IVacancyRepository vacancyRepository, ICategoryRepository categoryRepository, ICityRepository cityRepository, ICompanyRepository companyRepository, IDepartmentRepository departmentRepository, IMapper mapper, IBranchRepository branchRepository)
    {
        _vacancyRepository = vacancyRepository;
        _categoryRepository = categoryRepository;
        _cityRepository = cityRepository;
        _companyRepository = companyRepository;
        _departmentRepository = departmentRepository;
        _branchRepository = branchRepository;
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

    public async Task<BaseResponse<Pagination<VacancyGetDto>>> GetAllAsync(VacancyFilterDto filterDto, int pageNumber, int pageSize, bool isPaginated = true)
    {
		if (pageNumber < 1 || pageSize < 1)
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

        if(filterDto.IsRemote.HasValue)
        {
            query = query.Where(v => v.IsRemote);
        }

        if (filterDto.BranchId.HasValue)
        {
            query = query.Where(v => v.BranchId == filterDto.BranchId);
        }

        if(filterDto.CityId.HasValue)
        {
			query = query.Where(v => v.CityId == filterDto.CityId);
		}

        if (filterDto.CategoryId.HasValue)
        {
            query = query.Where(v => v.CategoryId == filterDto.CategoryId);
        }

        if (filterDto.CompanyId.HasValue)
        {
            query = query.Where(v => v.CompanyId == filterDto.CompanyId);
        }

        if(filterDto.DepartmentId.HasValue)
        {
            query = query.Where(v => v.DepartmentId == filterDto.DepartmentId);
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

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        var vacancyGetDtos = await query 
            .Select(v => new VacancyGetDto
            {
                Id = v.Id,
                Title = v.Title,
                VacancyType = v.VacancyType,
                JobLevel = v.JobLevel,
                ExpireDate = v.ExpireDate,
                CityName = v.City.Name,
                CategoryName = v.Category.Name,
                CompanyName = v.Company.Name,
                DepartmentName = v.Department.Name,
                BranchName = v.Branch.Name,
                IsRemote = v.IsRemote,
                IsSalaryVisible = v.IsSalaryVisible
            })
            .ToListAsync();

        var pagination = new Pagination<VacancyGetDto>
        {
            Items = vacancyGetDtos,
            TotalCount = totalItems,
            PageIndex = pageNumber,
            PageSize = pageSize,
            TotalPage = (int)Math.Ceiling((double)totalItems / pageSize)
        };

        return new BaseResponse<Pagination<VacancyGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Vacancies fetched successfully.",
            Data = pagination
        };
    }
    public async Task<BaseResponse<object>> CreateAsync(VacancyCreateDto createVacancyDto)
    {
        var validator = new VacancyCreateValidator();
        var validationResult = await validator.ValidateAsync(createVacancyDto);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

        var branch = await _branchRepository.GetByIdAsync(createVacancyDto.BranchId);
		if (branch is null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "This branch does not exist."
			};
		}
		var category = await _categoryRepository.GetByIdAsync(createVacancyDto.CategoryId);
        if(category is null)
        {
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "This category does not exist."
			};
		}
        var city = await _cityRepository.GetByIdAsync(createVacancyDto.CityId);
        if(city is null)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "This city does not exist."
            };
        }
        var company = await _companyRepository.GetByIdAsync(createVacancyDto.CompanyId);
        if(company is null)
        {
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "This company does not exist."
			};
		}
        var department = await _departmentRepository.GetByIdAsync(createVacancyDto.DepartmentId);
        if(department is null)
        {
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "This department does not exist."
			};
		}
        var createdVacancy = _mapper.Map<Vacancy>(createVacancyDto);
        await _vacancyRepository.AddAsync(createdVacancy);
        await _vacancyRepository.SaveChangesAsync();
        company.ExistedVacancy++;
        _companyRepository.Update(company);
        await _companyRepository.SaveChangesAsync();
        category.ExistedVacancy++;
        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync();
        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.Created,
            Message = "The vacancy has been successfully created."
        };
    }

    public async Task<BaseResponse<object>> UpdateAsync(Guid id, VacancyUpdateDto vacancyUpdateDto)
    {
        if (id != vacancyUpdateDto.Id)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Id does not match with the route parameter."
            };
        }

        var validator = new VacancyUpdateValidator();
        var validationResult = await validator.ValidateAsync(vacancyUpdateDto);

        if (!validationResult.IsValid)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var vacancy = await _vacancyRepository.GetByIdAsync(id);
        if (vacancy == null || vacancy.IsDeleted)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The vacancy does not exist."
            };
        }

		var branch = await _branchRepository.GetByIdAsync(vacancyUpdateDto.BranchId);
		if (branch is null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "This branch does not exist."
			};
		}
		var category = await _categoryRepository.GetByIdAsync(vacancyUpdateDto.CategoryId);
		if (category is null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "This category does not exist."
			};
		}
		var city = await _cityRepository.GetByIdAsync(vacancyUpdateDto.CityId);
		if (city is null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "This city does not exist."
			};
		}
		var company = await _companyRepository.GetByIdAsync(vacancyUpdateDto.CompanyId);
		if (company is null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "This company does not exist."
			};
		}
		var department = await _departmentRepository.GetByIdAsync(vacancyUpdateDto.DepartmentId);
		if (department is null)
		{
			return new BaseResponse<object>
			{
				StatusCode = HttpStatusCode.NotFound,
				Message = "This department does not exist."
			};
		}
        if(category.Id != vacancy.CategoryId)
        {
            var previousCategory = await _categoryRepository.GetByIdAsync(vacancy.CategoryId);
            previousCategory!.ExistedVacancy--;
            _categoryRepository.Update(previousCategory);
            category.ExistedVacancy++;
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
        }
		if (company.Id != vacancy.CompanyId)
		{
			var previousCompany = await _companyRepository.GetByIdAsync(vacancy.CompanyId);
			previousCompany!.ExistedVacancy--;
            _companyRepository.Update(previousCompany);
			company.ExistedVacancy++;
            _companyRepository.Update(company);
			await _companyRepository.SaveChangesAsync();
		}
		vacancy.Title = vacancyUpdateDto.Title;
        vacancy.VacancyType = vacancyUpdateDto.VacancyType;
        vacancy.JobLevel = vacancyUpdateDto.JobLevel;
        vacancy.ExpireDate = vacancyUpdateDto.ExpireDate;
        vacancy.CategoryId = vacancyUpdateDto.CategoryId;
        vacancy.CompanyId = vacancyUpdateDto.CompanyId;
        vacancy.CityId = vacancyUpdateDto.CityId;
        vacancy.BranchId = vacancyUpdateDto.BranchId;
        vacancy.DepartmentId = vacancyUpdateDto.DepartmentId;
        await _vacancyRepository.SaveChangesAsync();


        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.NoContent,
            Message = "The vacancy has been successfully updated."
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
        var company = await _companyRepository.GetByIdAsync(vacancy.CompanyId);
        var category = await _categoryRepository.GetByIdAsync(vacancy.CategoryId);
		company!.ExistedVacancy--;
        _companyRepository.Update(company);
		await _companyRepository.SaveChangesAsync();
		category!.ExistedVacancy--;
        _categoryRepository.Update(category);
		await _categoryRepository.SaveChangesAsync();
		return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.NoContent,
            Message = "The vacancy has been successfully deleted."
        };
    }
}




