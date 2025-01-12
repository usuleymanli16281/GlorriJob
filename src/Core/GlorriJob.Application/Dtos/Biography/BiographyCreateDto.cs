using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.Biography
{
	
	public record BiographyCreateDto
	{
		public IFormFile? Icon { get; set; }
		public required string Key { get; set; }
		public required string Value { get; set; }
		public Guid CompanyId { get; set; }
	}
}
