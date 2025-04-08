namespace GlorriJob.Application.Dtos.Industry
{
    public record IndustryUpdateDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
