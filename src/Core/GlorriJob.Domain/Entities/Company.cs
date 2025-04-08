using GlorriJob.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Domain.Entities
{
	public class Company : BaseEntity
	{
		public string Name { get; set; } = null!;
		public int EmployeeCount { get; set; }
		public int FoundedYear { get; set; }
		public string LogoPath { get; set; } = null!;
		public string? PosterPath { get; set; }
		public int ExistedVacancy { get; set; }
		public CompanyDetail CompanyDetail { get; set; } = null!;
		public Guid IndustryId { get; set; }
		public Industry Industry { get; set; } = null!;
		public ICollection<Biography> Biographies { get; set; } = null!;
		public ICollection<Branch> Branches { get; set; } = null!;
		public ICollection<Department> Departments { get; set; } = null!;
		public ICollection<Vacancy> Vacancies { get; set; } = null!;
	}
}
