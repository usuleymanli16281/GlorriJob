namespace GlorriJob.Application.Dtos.Branch;

public record BranchCreateDto
{
    public required string Name { get; set; }
    public required string Location { get; set; }
    public bool IsMain { get; set; }
    public Guid CityId { get; set; }
    public Guid CompanyId { get; set; }
    public IEnumerable<Guid> DepartmentIds { get; init; } = Enumerable.Empty<Guid>();
    public IEnumerable<Guid> VacancyIds { get; init; } = Enumerable.Empty<Guid>();
}


