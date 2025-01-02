using FluentValidation;
using GlorriJob.Application.Dtos.Company;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Validations.Company
{
	public class CompanyCreateValidator : AbstractValidator<CompanyCreateDto>
	{
		public CompanyCreateValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty()
				.WithMessage("Name is required.");

			RuleFor(x => x.Logo)
				.NotEmpty()
				.WithMessage("Logo is required.")
				.Must(ValidateImage)
				.WithMessage("The uploaded file is not a valid image.")
				.Must(file => file.Length > 0)
				.WithMessage("The file is empty.");

			RuleFor(x => x.Poster)
				.Must(ValidateImage!)
				.When(x => x.Poster is not null)
				.WithMessage("The uploaded file is not a valid image.")
				.Must(file => file!.Length > 0)
				.When(x => x.Poster is not null)
				.WithMessage("The file is empty.");

			RuleFor(x => x.EmployeeCount)
				.GreaterThanOrEqualTo(1)
				.WithMessage("Employee count should be at least 1.");

			RuleFor(x => x.FoundedYear)
				.LessThanOrEqualTo(DateTime.Now.Year)
				.WithMessage("Founded year should be less than current year");

		}
		private bool ValidateImage(IFormFile file)
		{
			if (file is null) return false;

			var imageMimeTypes = new List<string>
			{
				"image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp", "image/svg+xml"
			};

			return imageMimeTypes.Contains(file.ContentType.ToLower());
		}
	}
}
