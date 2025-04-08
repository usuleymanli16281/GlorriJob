using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.Company
{
	public record CompanyCreateDto
	{
		public required string Name { get; set; }
		public int EmployeeCount { get; set; }
		public int FoundedYear { get; set; }
		public IFormFile? Poster { get; set; }
		public required IFormFile Logo { get; set; }
		public required Guid IndustryId { get; set; }
	}
}
