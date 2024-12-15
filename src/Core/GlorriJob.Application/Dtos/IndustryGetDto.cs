namespace GlorriJob.Application.Dtos
{
    public  record  IndustryGetDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }


    }
}
