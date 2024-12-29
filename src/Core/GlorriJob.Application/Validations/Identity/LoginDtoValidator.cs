using FluentValidation;
using GlorriJob.Application.Dtos.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Validations.Identity
{
	public class LoginDtoValidator : AbstractValidator<LoginDto>
	{
		public LoginDtoValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress()
				.MinimumLength(8)
				.WithName("Email");
			RuleFor(x => x.Password)
				.NotEmpty()
				.WithName("Password");
		}
	}
}
