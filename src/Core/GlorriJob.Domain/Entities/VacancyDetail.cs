using GlorriJob.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Domain.Entities
{
	public class VacancyDetail : BaseEntity
	{
		public VacancyType VacancyType { get; set; } = VacancyType.FullTime;
		public string Content { get; set; } = null!;
		public Guid VacancyId { get; set; }
		public Vacancy Vacancy { get; set; } = null!;
		public string? Salary { get; set; }
	}
}
