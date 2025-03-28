﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace GlorriJob.Common.Shared
{
	public class ErrorDetail
	{
		public int StatusCode { get; set; }
		public string? Message { get; set; }
		public string? Detail { get; set; }
		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
