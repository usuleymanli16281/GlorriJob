using FluentValidation;
using GlorriJob.Application.Dtos;

namespace GlorriJob.Application.Validations.City
{
	public class CityCreateValidator : AbstractValidator<CityCreateDto>
	{
        public CityCreateValidator()
        {
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Name is required.")
				.MaximumLength(50).WithMessage("Name can not exceed 50 characters.");
		}
    }
}
