using FluentValidation;
using GlorriJob.Application.Dtos.Biography;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Validations.Biography
{
	public class BiographyUpdateValidator : AbstractValidator<BiographyUpdateDto>
	{
        public BiographyUpdateValidator()
        {
			RuleFor(x => x.Key)
				.NotEmpty()
				.WithMessage("Key is required.");

			RuleFor(x => x.Value)
				.NotEmpty()
				.WithMessage("Value is required");

			RuleFor(x => x.Icon)
				.Must(ValidateImage!)
				.When(x => x.Icon is not null)
				.WithMessage("The uploaded file is not a valid image.")
				.Must(file => file!.Length > 0)
				.When(x => x.Icon is not null)
				.WithMessage("The file is empty.");
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
