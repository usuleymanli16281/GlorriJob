using GlorriJob.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Domain.Entities
{
	public class CompanyDetail : BaseEntity
	{
		public string Content { get; set; } = null!;
		public Guid CompanyId { get; set; }
		public Company Company { get; set; } = null!;
	}
}
