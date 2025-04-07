using GlorriJob.Domain;

namespace GlorriJob.Application.Dtos.Vacancy;

public record VacancyFilterDto
{
	public string? Title { get; set; }
	public VacancyType? VacancyType { get; set; }
	public JobLevel? JobLevel { get; set; }
	public Guid? CategoryId { get; set; }
	public Guid? CompanyId { get; set; }
	public Guid? BranchId { get; set; }
	public Guid? DepartmentId { get; set; }
	public Guid? CityId { get; set; }
	public Guid? VacancyDetailId { get; set; }
	public bool? IsRemote { get; set; }
	public DateTime? ExpireDateFrom { get; set; }
	public DateTime? ExpireDateTo { get; set; }
}


