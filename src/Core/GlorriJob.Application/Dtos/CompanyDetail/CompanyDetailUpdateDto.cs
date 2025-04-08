using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.CompanyDetail
{
	public record CompanyDetailUpdateDto
	{
		public Guid Id { get; set; }
		public required string Content { get; set; }
		public Guid CompanyId { get; set; }
	}
}
