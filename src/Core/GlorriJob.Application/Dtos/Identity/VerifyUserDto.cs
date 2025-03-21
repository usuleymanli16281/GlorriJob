using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.Identity
{
	public record VerifyUserDto
	{
		public required string Email { get; set; }
		public required string Otp {  get; set; }
	}
}
