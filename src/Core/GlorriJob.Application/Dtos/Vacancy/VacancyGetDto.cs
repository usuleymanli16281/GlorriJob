using GlorriJob.Domain;

namespace GlorriJob.Application.Dtos.Vacancy;

public record VacancyGetDto
{
   public Guid Id { get; set; }
    public required string Title { get; set; } = null!;
    public VacancyType VacancyType { get; set; } = VacancyType.FullTime;
    public JobLevel JobLevel { get; set; } = JobLevel.EntryLevel;
    public required string CategoryName { get; set; }
    public required string CompanyName { get; set; }
    public required string BranchName { get; set; }
    public required string DepartmentName { get; set; }
    public required string CityName { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsSalaryVisible { get; set; }
    public bool IsRemote { get; set; }
    public int ViewCount { get; set; }
}