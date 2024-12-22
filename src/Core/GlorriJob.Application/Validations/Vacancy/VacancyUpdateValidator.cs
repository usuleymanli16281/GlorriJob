using FluentValidation;
using GlorriJob.Application.Dtos;
using GlorriJob.Application.Dtos.Vacancy;

namespace GlorriJob.Application.Validations.Vacancy;

public class VacancyUpdateValidator : AbstractValidator<VacancyUpdateDto>
{
    public VacancyUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Vacancy Id is required.");

        RuleFor(x => x.Title)
             .NotEmpty().When(x => !string.IsNullOrEmpty(x.Title))
             .WithMessage("Title cannot be empty if provided.")
             .Length(3, 100).WithMessage("Title must be between 3 and 100 characters.");

        RuleFor(x => x.VacancyType)
            .IsInEnum().WithMessage("Invalid Vacancy Type.");

        RuleFor(x => x.JobLevel)
            .IsInEnum().WithMessage("Invalid Job Level.");

        RuleFor(x => x.IsSalaryVisible)
            .NotNull().WithMessage("Salary visibility status is required.");

        RuleFor(x => x.IsRemote)
            .NotNull().WithMessage("Remote status is required.");

        RuleFor(x => x.ExpireDate)
            .GreaterThan(DateTime.Now).WithMessage("Expire date must be in the future.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().When(x => x.CategoryId.HasValue).WithMessage("CategoryId cannot be empty if provided.");

        RuleFor(x => x.CompanyId)
            .NotEmpty().When(x => x.CompanyId.HasValue).WithMessage("CompanyId cannot be empty if provided.");

        RuleFor(x => x.BranchId)
            .NotEmpty().When(x => x.BranchId.HasValue).WithMessage("BranchId cannot be empty if provided.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().When(x => x.DepartmentId.HasValue).WithMessage("DepartmentId cannot be empty if provided.");

        RuleFor(x => x.CityId)
            .NotEmpty().When(x => x.CityId.HasValue).WithMessage("CityId cannot be empty if provided.");

        RuleFor(x => x.VacancyDetail)
            .NotNull().WithMessage("Vacancy detail is required.");
    }
}

