using FluentValidation;
using GlorriJob.Application.Dtos.VacancyDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Validations.VacancyDetail
{
	public class VacancyDetailGetValidator : AbstractValidator<VacancyDetailGetDto>
	{
        public VacancyDetailGetValidator()
        {
			RuleFor(x => x.Id)
		   .NotEmpty().WithMessage("Id is required.");

			RuleFor(x => x.VacancyType)
				.IsInEnum()
				.WithMessage("Invalid Vacancy Type.");

			RuleFor(x => x.Content)
				.NotEmpty().WithMessage("Content is required.");

			RuleFor(x => x.VacancyId)
				.NotEmpty().WithMessage("VacancyId is required.");

			RuleFor(x => x.Salary)
				.Matches(@"^\d+(\.\d{1,2})?$").When(x => !string.IsNullOrWhiteSpace(x.Salary))
				.WithMessage("Salary must be a valid number.");
		}
    }
}
