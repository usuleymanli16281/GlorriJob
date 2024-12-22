using GlorriJob.Application.Dtos.Vacancy;
using GlorriJob.Domain;

namespace GlorriJob.Application.Dtos.VacancyDetail;

public record VacancyDetailCreateDto
{
    public required VacancyType VacancyType { get; set; }
    public required string Content { get; set; } = null!;
    public string? Salary { get; set; }
    public required Guid VacancyId { get; set; }
}

