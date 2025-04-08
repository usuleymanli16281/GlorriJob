namespace GlorriJob.Application.Dtos.Industry
{
    public record IndustryGetDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }



    }
}
