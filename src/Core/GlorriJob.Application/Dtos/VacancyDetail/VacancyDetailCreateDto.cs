using GlorriJob.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.VacancyDetail
{
	public record VacancyDetailCreateDto
	{
		public VacancyType VacancyType { get; set; } = VacancyType.FullTime;
		public required string Content { get; set; }
		public Guid VacancyId { get; set; }
		public string? Salary { get; set; }
	}
}
