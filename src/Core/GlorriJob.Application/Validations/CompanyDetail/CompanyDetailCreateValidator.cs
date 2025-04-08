using FluentValidation;
using GlorriJob.Application.Dtos.CompanyDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Validations.CompanyDetail
{
	public class CompanyDetailCreateValidator : AbstractValidator<CompanyDetailCreateDto>
	{
		public CompanyDetailCreateValidator()
		{
			RuleFor(x => x.Content)
				.NotEmpty().WithMessage("Content is required.")
				.MaximumLength(1000).WithMessage("Content cannot exceed 1000 characters.");

			RuleFor(x => x.CompanyId)
				.NotEmpty().WithMessage("CompanyId is required.");
		}
	}
}
