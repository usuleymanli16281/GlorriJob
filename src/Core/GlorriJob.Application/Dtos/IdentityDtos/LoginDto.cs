namespace GlorriJob.Application.Dtos.IdentityDtos;

public record LoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
