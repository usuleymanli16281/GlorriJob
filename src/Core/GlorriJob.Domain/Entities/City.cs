using GlorriJob.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Domain.Entities
{
	public class City : BaseEntity
	{
		public string Name { get; set; } = null!;
		public ICollection<Branch> Branches { get; set; } = null!;
		public ICollection<Vacancy> Vacancies { get; set; } = null!;
	}
}
