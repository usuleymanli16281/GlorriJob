namespace GlorriJob.Application.Dtos;

public record CityGetDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
