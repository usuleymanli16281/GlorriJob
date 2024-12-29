﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using GlorriJob.Application.Dtos.Department;

public class DepartmentUpdateValidator : AbstractValidator<DepartmentUpdateDto>
{
    public DepartmentUpdateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.BranchId)
            .NotEqual(Guid.Empty).WithMessage("BranchId is required.");
    }
}

