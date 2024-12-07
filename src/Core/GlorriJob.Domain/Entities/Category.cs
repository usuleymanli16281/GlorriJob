using GlorriJob.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Domain.Entities
{
	public class Category : BaseEntity
	{
		public string Name { get; set; } = null!;
		public int ExistedVacancy { get; set; }
		public ICollection<Vacancy> Vacancies { get; set; } = null!;
	}
}
