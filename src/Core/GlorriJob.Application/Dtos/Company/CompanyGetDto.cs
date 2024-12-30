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
		public Guid Id { get; init; }
		public required string Name { get; init; }
		public int EmployeeCount { get; init; }
		public int FoundedYear { get; init; }
		public string? PosterPath {  get; init; } 
		public required string LogoPath { get; init; }
		public required IndustryGetDto IndustryGetDto { get; init; }
	}
}
