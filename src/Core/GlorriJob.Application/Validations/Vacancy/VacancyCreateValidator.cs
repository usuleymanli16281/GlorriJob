
using FluentValidation;
using GlorriJob.Application.Dtos.Vacancy;

namespace GlorriJob.Application.Validations.Vacancy;
public class VacancyCreateValidator : AbstractValidator<VacancyCreateDto>
{
    public VacancyCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(3, 100).WithMessage("Title must be between 3 and 100 characters.");

        RuleFor(x => x.VacancyType)
            .IsInEnum().WithMessage("Invalid Vacancy Type.");

        RuleFor(x => x.JobLevel)
            .IsInEnum().WithMessage("Invalid Job Level.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Company is required.");

        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("Branch is required.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department is required.");

        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.ExpireDate)
            .GreaterThan(DateTime.Now).WithMessage("Expire date must be in the future.");

        RuleFor(x => x.IsSalaryVisible)
            .NotNull().WithMessage("Salary visibility status is required.");

        RuleFor(x => x.IsRemote)
            .NotNull().WithMessage("Remote status is required.");
    }
}

