﻿namespace GlorriJob.Application.Validations.Vacancy;
using FluentValidation;
using GlorriJob.Application.Dtos.Vacancy;

public class VacancyCreateValidator : AbstractValidator<VacancyCreateDto>
{
    public VacancyCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(5, 200).WithMessage("Title should be between 5 and 200 characters.");

        RuleFor(x => x.VacancyType)
            .IsInEnum().WithMessage("Invalid Vacancy Type.");

        RuleFor(x => x.JobLevel)
            .IsInEnum().WithMessage("Invalid Job Level.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required.");

        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("BranchId is required.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("DepartmentId is required.");

        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("CityId is required.");

        RuleFor(x => x.ExpireDate)
            .GreaterThan(DateTime.Now).WithMessage("ExpireDate must be in the future.");

        RuleFor(x => x.IsSalaryVisible)
            .NotNull().WithMessage("IsSalaryVisible is required.");

        RuleFor(x => x.IsRemote)
            .NotNull().WithMessage("IsRemote is required.");
    }
}

