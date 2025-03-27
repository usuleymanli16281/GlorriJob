using FluentValidation;
using GlorriJob.Application.Dtos.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Validations.Branch
{
	public class BranchUpdateValidator : AbstractValidator<BranchUpdateDto>
	{
        public BranchUpdateValidator()
        {
			RuleFor(x => x.Id)
			.NotEmpty().WithMessage("Id is required.")
			.NotEqual(Guid.Empty).WithMessage("Id must be a valid GUID.");

			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Branch name is required.")
				.MaximumLength(100).WithMessage("Branch name must not exceed 100 characters.");

			RuleFor(x => x.Location)
				.NotEmpty().WithMessage("Location is required.")
				.MaximumLength(200).WithMessage("Location must not exceed 200 characters.");

			RuleFor(x => x.CityId)
				.NotEmpty().WithMessage("CityId is required.")
				.NotEqual(Guid.Empty).WithMessage("CityId must be a valid GUID.");

			RuleFor(x => x.CompanyId)
				.NotEmpty().WithMessage("CompanyId is required.")
				.NotEqual(Guid.Empty).WithMessage("CompanyId must be a valid GUID.");
		}
    }
}
