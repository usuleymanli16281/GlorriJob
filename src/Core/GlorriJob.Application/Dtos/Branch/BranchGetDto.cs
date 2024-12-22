using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Company;
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Application.Dtos.Vacancy;

namespace GlorriJob.Application.Dtos.Branch;

public record BranchGetDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public bool IsMain { get; set; }
    public required Guid CityId { get; init; }
    public required string CityName { get; init; }
    public required CompanyGetDto Company { get; init; }
    public IEnumerable<DepartmentGetDto> Departments { get; init; } = Enumerable.Empty<DepartmentGetDto>();
    public IEnumerable<VacancyGetDto> Vacancies { get; init; } = Enumerable.Empty<VacancyGetDto>();
}

