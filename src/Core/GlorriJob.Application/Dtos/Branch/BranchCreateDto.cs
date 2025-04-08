using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.Branch
{
	public record BranchCreateDto
	{
		public string Name { get; set; } = null!;
		public string Location { get; set; } = null!;
		public bool IsMain { get; set; }
		public Guid CityId { get; set; }
		public Guid CompanyId { get; set; }
	}
}
