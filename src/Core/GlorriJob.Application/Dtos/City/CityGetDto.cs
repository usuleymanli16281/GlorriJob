namespace GlorriJob.Application.Dtos.City;

public record CityGetDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
