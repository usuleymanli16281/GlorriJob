using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Application.Dtos.City;
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Application.Dtos.Branch;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Application.Dtos.Company;
using GlorriJob.Domain;

namespace GlorriJob.Application.Dtos;

public record VacancyUpdateDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; } = null!;
    public VacancyType? VacancyType { get; set; }
    public JobLevel? JobLevel { get; set; } 
    public bool? IsSalaryVisible { get; set; }
    public bool? IsRemote { get; set; }
    public DateTime? ExpireDate { get; set; }

    public Guid? CategoryId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CityId { get; set; }
    public Guid? CompanyId { get; set; }

    public VacancyDetailUpdateDto? VacancyDetail { get; set; } = null!;
}

