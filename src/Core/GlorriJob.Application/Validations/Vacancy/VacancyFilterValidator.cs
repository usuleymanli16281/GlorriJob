namespace GlorriJob.Application.Validations.Vacancy;

using System.Linq;
using FluentValidation;
using GlorriJob.Application.Dtos.Vacancy;
using GlorriJob.Domain;

public class VacancyFilterValidator : AbstractValidator<VacancyFilterDto>
{
    public VacancyFilterValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0.");

        RuleFor(x => x.SortBy)
            .Must(sort => sort != null && sort.Any()).WithMessage("At least one sorting option is required.");

        RuleForEach(x => x.SortBy)
            .Must(sort => new List<string> { "ExpireDate", "Title", "ViewCount" }.Contains(sort))
            .WithMessage("Invalid sorting option.");

        RuleFor(x => x.AdditionalFilters)
            .Must(filters => filters == null || filters.Count <= 10).WithMessage("You can provide up to 10 additional filters.");

        RuleFor(x => x.Title)
            .Must(title => string.IsNullOrEmpty(title) || title.Length >= 3).WithMessage("Title must be at least 3 characters long or null.");

        RuleFor(x => x.VacancyType)
            .Must(vacancyType => !vacancyType.HasValue || Enum.IsDefined(typeof(VacancyType), vacancyType.Value))
            .WithMessage("Invalid VacancyType. Valid options are FullTime, PartTime, or Internship.");
    }


}

