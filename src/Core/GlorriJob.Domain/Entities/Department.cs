using GlorriJob.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Domain.Entities
{
	public class Department : BaseEntity
	{
		public string Name { get; set; } = null!;
		public Guid BranchId { get; set; }
		public Branch Branch { get; set; } = null!;
		public ICollection<Vacancy> Vacancies { get; set; } = null!;
	}
}
