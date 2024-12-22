using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Application.Dtos.Branch;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Application.Dtos.Company;
using GlorriJob.Domain;
using GlorriJob.Domain.Entities;

namespace GlorriJob.Application.Dtos.Vacancy;

public record VacancyGetDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; } = null!;
    public VacancyType VacancyType { get; set; } = VacancyType.FullTime;
    public JobLevel JobLevel { get; set; } = JobLevel.EntryLevel;
    public bool IsSalaryVisible { get; set; }
    public bool IsRemote { get; set; }
    public DateTime ExpireDate { get; set; }
    public int ViewCount { get; set; }

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;

    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;

    public Guid CityId { get; set; }
    public string CityName { get; set; } = null!;

    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = null!;

    public BranchGetDto Branch { get; set; } = null!;
    public VacancyDetailGetDto VacancyDetail { get; set; } = null!;
}
