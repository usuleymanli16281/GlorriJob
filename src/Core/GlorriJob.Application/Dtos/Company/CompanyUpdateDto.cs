using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.Company
{
	public record CompanyUpdateDto
	{
		public Guid Id { get; init; }
		public required string Name { get; set; }
		public int EmployeeCount { get; set; }
		public int FoundedYear { get; set; }
		public IFormFile? Poster { get; set; }
		public required IFormFile Logo { get; set; }
		public Guid IndustryId {  get; set; }
	}
}
