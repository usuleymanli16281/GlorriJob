namespace GlorriJob.Application.Dtos.Category;

public record CategoryUpdateDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
}

