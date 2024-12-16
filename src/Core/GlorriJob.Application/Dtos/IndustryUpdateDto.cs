namespace GlorriJob.Application.Dtos
{
    public  record  IndustryUpdateDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
