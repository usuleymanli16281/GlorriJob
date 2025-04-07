using GlorriJob.Application.Dtos.Department;
using GlorriJob.Application.Dtos.Industry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.Company
{
	public record CompanyGetDto
	{
		public Guid Id { get; set; }
		public required string Name { get; set; }
		public int ExistedVacancy { get; set; }
		public int EmployeeCount { get; set; }
		public int FoundedYear { get; set; }
		public string? PosterPath {  get; set; } 
		public required string LogoPath { get; set; }
		public required string IndustryName { get; set; }
	}
}
