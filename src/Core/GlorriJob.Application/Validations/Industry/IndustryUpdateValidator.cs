using FluentValidation;
using GlorriJob.Application.Dtos.Industry;

namespace GlorriJob.Application.Validations.Industry;

public class IndustryUpdateValidator : AbstractValidator<IndustryUpdateDto>
{
    public IndustryUpdateValidator()
    {
        RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Industry name is required.")
               .Length(2, 100).WithMessage("Industry name must be between 2 and 100 characters.");
    }
}
