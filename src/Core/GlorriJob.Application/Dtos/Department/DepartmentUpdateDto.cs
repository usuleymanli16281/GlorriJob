﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.Department
{
	public record DepartmentUpdateDto
	{
		public Guid Id { get; set; }
		public required string Name { get; set; } = string.Empty;
		public Guid BranchId { get; set; }
	}
}
