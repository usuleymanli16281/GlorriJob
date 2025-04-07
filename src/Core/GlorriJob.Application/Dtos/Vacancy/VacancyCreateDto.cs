using GlorriJob.Domain;

namespace GlorriJob.Application.Dtos.Vacancy;


public record VacancyCreateDto
{
    public required string Title { get; set; }
    public required VacancyType VacancyType { get; set; } = VacancyType.FullTime;
    public required JobLevel JobLevel { get; set; } = JobLevel.EntryLevel;
    public required Guid CategoryId { get; set; }
    public required Guid CompanyId { get; set; }
    public required Guid BranchId { get; set; }
    public required Guid DepartmentId { get; set; }
    public required Guid CityId { get; set; }
    public required DateTime ExpireDate { get; set; }
    public bool IsSalaryVisible { get; set; }
    public bool IsRemote { get; set; }
}


