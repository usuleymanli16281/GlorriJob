using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Domain.Entities.Common
{
	public class BaseEntity
	{
		public Guid Id { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ModifiedDate { get; set;}
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }	
		public bool IsDeleted { get; set;}
	}
}
