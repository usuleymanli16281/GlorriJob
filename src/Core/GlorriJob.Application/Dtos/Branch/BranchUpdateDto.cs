namespace GlorriJob.Application.Dtos.Branch;

public record BranchUpdateDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public bool? IsMain { get; set; }
    public Guid? CityId { get; set; }
    public Guid? CompanyId { get; set; }
    public IEnumerable<Guid>? DepartmentIds { get; set; }
    public IEnumerable<Guid>? VacancyIds { get; set; }
}


