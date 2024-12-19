namespace GlorriJob.Application.Dtos.CityDtos;

public record CityGetDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
