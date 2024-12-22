using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using GlorriJob.Application.Dtos.Branch;

namespace GlorriJob.Application.Validations.Branch;

public class BranchUpdateValidator : AbstractValidator<BranchUpdateDto>
{
    public BranchUpdateValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Name cannot be longer than 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Location)
            .MaximumLength(500).WithMessage("Location cannot be longer than 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Location));

        RuleFor(x => x.CityId)
            .Must(BeValidGuid).WithMessage("CityId must be a valid GUID.")
            .When(x => x.CityId.HasValue);

        RuleFor(x => x.CompanyId)
            .Must(BeValidGuid).WithMessage("CompanyId must be a valid GUID.")
            .When(x => x.CompanyId.HasValue);

        RuleForEach(x => x.DepartmentIds)
             .Must(BeValidGuid).WithMessage("Each DepartmentId must be a valid GUID.")
             .When(x => x.DepartmentIds != null && x.DepartmentIds.Any());

        RuleForEach(x => x.VacancyIds)
            .Must(BeValidGuid).WithMessage("Each VacancyId must be a valid GUID.")
            .When(x => x.VacancyIds != null && x.VacancyIds.Any());
    }

    private bool BeValidGuid(Guid? guid)
    {
        return guid.HasValue && guid.Value != Guid.Empty;
    }

    private bool BeValidGuid(Guid guid)
    {
        return guid != Guid.Empty;
    }

}

