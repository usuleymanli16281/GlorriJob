using GlorriJob.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Domain.Entities
{
	public class Vacancy : BaseEntity
	{
		public string Title { get; set; } = null!;
		public VacancyType VacancyType { get; set; } = VacancyType.FullTime;
		public JobLevel JobLevel { get; set; } = JobLevel.EntryLevel;
		public Guid CategoryId { get; set; }
		public Category Category { get; set; } = null!;
		public Guid CompanyId { get; set; }
		public Company Company { get; set; } = null!;
		public Guid BranchId { get; set; }
		public Branch Branch { get; set; } = null!;
		public Guid DepartmentId { get; set; }
		public Department Department { get; set; } = null!;
		public Guid CityId { get; set; }
		public City City { get; set; } = null!;
        public VacancyDetail VacancyDetail { get; set; } = null!;
		public DateTime ExpireDate { get; set; }
		public bool IsSalaryVisible { get; set; }
		public bool IsRemote { get; set; }
		public int ViewCount { get; set; }
	}
}
