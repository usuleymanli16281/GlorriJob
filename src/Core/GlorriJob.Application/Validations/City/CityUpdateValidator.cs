using FluentValidation;
using GlorriJob.Application.Dtos.City;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Validations.City
{
    public class CityUpdateValidator : AbstractValidator<CityUpdateDto>
	{
		public CityUpdateValidator() 
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Name is required.")
				.MaximumLength(50).WithMessage("Name can not exceed 50 characters.");
		}
	}
}
