using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.Identity
{
	public record ForgotPasswordDto
	{
		public required string Email { get; set; }
	}
}
