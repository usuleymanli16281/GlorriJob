using FluentValidation;
using GlorriJob.Application.Dtos.Category;

namespace GlorriJob.Application.Validations.Category;

public class CategoryCreateValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Name is required.")
           .Length(2, 100).WithMessage("Name should be between 2 and 100 characters.");
    }
}
