using GlorriJob.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Domain.Entities
{
	public class Branch : BaseEntity
	{
		public string Name { get; set; } = null!;
		public string Location { get; set; } = null!;
		public bool IsMain { get; set; }
		public Guid CityId { get; set; }
		public City City { get; set; } = null!;
		public Guid CompanyId { get; set; }
		public Company Company { get; set; } = null!;
		public ICollection<Department> Departments { get; set; } = null!;
		public ICollection<Vacancy> Vacancies { get; set; } = null!;
	}
}
