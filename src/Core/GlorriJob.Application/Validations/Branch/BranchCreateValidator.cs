using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using GlorriJob.Application.Dtos.Branch;

namespace GlorriJob.Application.Validations.Branch;

public class BranchCreateValidator : AbstractValidator<BranchCreateDto>
{
    public BranchCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(1, 100).WithMessage("Name must be between 1 and 100 characters.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .Length(1, 255).WithMessage("Location must be between 1 and 255 characters.");

        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("CityId is required.");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required.");

        RuleForEach(x => x.DepartmentIds)
            .NotEmpty().WithMessage("DepartmentId cannot be empty.");

        RuleForEach(x => x.VacancyIds)
            .NotEmpty().WithMessage("VacancyId cannot be empty.");
    }
}

