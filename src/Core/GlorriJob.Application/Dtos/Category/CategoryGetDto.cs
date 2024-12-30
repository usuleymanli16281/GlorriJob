namespace GlorriJob.Application.Dtos.Category;

public record CategoryGetDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public int ExistedVacancy { get; init; }
}

