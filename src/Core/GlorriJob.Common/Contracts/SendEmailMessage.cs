﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Common.Contracts
{
	public class SendEmailMessage
	{
		public required string To { get; set; }
		public required string Subject { get; set; }
		public required string Body { get; set; }
	}
}
