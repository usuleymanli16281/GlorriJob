using FluentValidation;
using GlorriJob.Application.Dtos.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Validations.Identity
{
	public class RegisterDtoValidator : AbstractValidator<RegisterDto>
	{
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3)
                .WithName("Name");
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MinimumLength(8)
                .WithName("Email");
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithName("Password");
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password)
                .WithName("Confirm Password");
            
        }
    }
}
