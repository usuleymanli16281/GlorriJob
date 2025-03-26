using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.CompanyDetail
{
	public record CompanyDetailGetDto
	{
		public Guid Id { get; set; }
		public required string Content { get; set; }
		public Guid CompanyId { get; set; }
	}
}
