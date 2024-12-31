using FluentValidation;
using GlorriJob.Application.Dtos.Company;
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
            
            RuleFor(x => x.LogoPath)
                .NotEmpty()
                .WithMessage("Logo is required.");
            
            RuleFor(x => x.EmployeeCount)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Employee count should be at least 1.");
            
            RuleFor(x => x.FoundedYear)
                .LessThanOrEqualTo(DateTime.Now.Year)
                .WithMessage("Founded year should be less than current year");
                
        }
    }
}
