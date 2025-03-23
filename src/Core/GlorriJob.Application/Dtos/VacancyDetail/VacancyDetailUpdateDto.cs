using GlorriJob.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.VacancyDetail
{
	public record VacancyDetailUpdateDto
	{
		public Guid Id { get; set; }
		public VacancyType VacancyType { get; set; } = VacancyType.FullTime;
		public string Content { get; set; } = null!;
		public Guid VacancyId { get; set; }
		public string? Salary { get; set; }
	}
}
