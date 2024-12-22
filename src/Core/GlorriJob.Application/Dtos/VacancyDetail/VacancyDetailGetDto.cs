using GlorriJob.Application.Dtos.Vacancy;
using GlorriJob.Domain;

namespace GlorriJob.Application.Dtos.VacancyDetail;

public record VacancyDetailGetDto
{
    public Guid Id { get; set; }
    public VacancyType VacancyType { get; set; } = VacancyType.FullTime;
    public string Content { get; set; } = null!;
    public string? Salary { get; set; }
    public Guid VacancyId { get; set; }
}

