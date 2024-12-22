using FluentValidation;
using GlorriJob.Application.Dtos.Category;

namespace GlorriJob.Application.Validations.Category;

public class CategoryUpdateValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategoryUpdateValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 100).WithMessage("Name should be between 3 and 100 characters.");
    }
}