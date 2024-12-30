using FluentValidation;
using GlorriJob.Application.Dtos.Category;

namespace GlorriJob.Application.Validations.Category;

public class CategoryCreateValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Name is required.")
           .Length(3, 100).WithMessage("Name should be between 3 and 100 characters.");
    }
}
