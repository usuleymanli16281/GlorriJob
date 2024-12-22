using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlorriJob.Domain.Entities;

namespace GlorriJob.Application.Dtos.Company;

public  record  CompanyGetDto
{
    public string Name { get; set; } = null!;
    public int EmployeeCount { get; set; }
    public int FoundedYear { get; set; }
    public string LogoPath { get; set; } = null!;
    public string? PosterPath { get; set; } = null!;
    public int ExistedVacancy { get; set; }
    public CompanyDetail CompanyDetail { get; set; } = null!;
    public Guid IndustryId { get; set; }
}
