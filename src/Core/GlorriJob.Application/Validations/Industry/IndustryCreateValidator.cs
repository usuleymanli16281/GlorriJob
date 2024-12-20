using FluentValidation;
using GlorriJob.Application.Dtos.Industry;

namespace GlorriJob.Application.Validations.Industry;

public class IndustryCreateValidator : AbstractValidator<IndustryCreateDto>
{
    public IndustryCreateValidator()
    {
        RuleFor(x => x.Name)
              .NotEmpty().WithMessage("Industry name is required.")
              .Length(3, 100).WithMessage("Industry name must be between 3 and 100 characters.");
    }
}
