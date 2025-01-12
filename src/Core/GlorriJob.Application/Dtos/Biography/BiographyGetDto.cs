using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.Biography
{
	public record BiographyGetDto
	{
		public Guid Id { get; set; }
		public string? Icon { get; set; }
		public required string Key { get; set; }
		public required string Value { get; set; }
		public Guid CompanyId { get; set; }
	}
}
