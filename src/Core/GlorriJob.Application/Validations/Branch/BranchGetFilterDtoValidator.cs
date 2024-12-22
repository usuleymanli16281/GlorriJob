using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using GlorriJob.Application.Dtos.Branch;

namespace GlorriJob.Application.Validations.Branch
{
    public class BranchGetFilterDtoValidator : AbstractValidator<BranchFilterDto>
    {
        public BranchGetFilterDtoValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1).WithMessage("Page must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");

            RuleFor(x => x.SortBy)
                .Matches(@"^[a-zA-Z]+$").WithMessage("SortBy must be a valid string with only alphabetic characters.");

            RuleFor(x => x.SortOrder)
                .Matches("^(asc|desc)$").WithMessage("SortOrder must be either 'asc' or 'desc'.");
        }
    }

}
