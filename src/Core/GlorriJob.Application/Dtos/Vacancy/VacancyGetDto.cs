using GlorriJob.Domain;

namespace GlorriJob.Application.Dtos.Vacancy;

public record VacancyGetDto
{
   public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public VacancyType VacancyType { get; set; } = VacancyType.FullTime;
    public JobLevel JobLevel { get; set; } = JobLevel.EntryLevel;
    public Guid CategoryId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid BranchId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid CityId { get; set; }
    public Guid VacancyDetailId { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsSalaryVisible { get; set; }
    public bool IsRemote { get; set; }
    public int ViewCount { get; set; }
}