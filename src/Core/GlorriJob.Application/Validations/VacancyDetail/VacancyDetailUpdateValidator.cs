using FluentValidation;
using GlorriJob.Application.Dtos.VacancyDetail;

namespace GlorriJob.Application.Validations.VacancyDetail;

public class VacancyDetailUpdateValidator : AbstractValidator<VacancyDetailUpdateDto>
{
    public VacancyDetailUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.")
            .Must(id => id != Guid.Empty).WithMessage("Id must be a valid GUID.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .Length(10, 1000).WithMessage("Content should be between 10 and 1000 characters.");

        RuleFor(x => x.Salary)
            .Matches(@"^\d+(\.\d{1,2})?$").WithMessage("Salary must be a valid number with up to two decimal places.")
            .When(x => !string.IsNullOrEmpty(x.Salary));

        RuleFor(x => x.VacancyId)
            .NotEmpty().WithMessage("Vacancy ID is required.");

        RuleFor(x => x.VacancyType)
            .IsInEnum().WithMessage("VacancyType must be a valid type.");
    }
}

