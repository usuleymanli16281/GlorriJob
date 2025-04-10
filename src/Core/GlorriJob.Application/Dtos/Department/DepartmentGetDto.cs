﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.Department
{
	public record DepartmentGetDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public Guid BranchId {  get; set; }
	}
}
