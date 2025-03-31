using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Common.Contracts;
using MassTransit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Infrastructure.Consumers
{
	public class SendEmailConsumer : IConsumer<SendEmailMessage>
	{
		private IEmailService _emailService { get; }
        public SendEmailConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<SendEmailMessage> context)
		{
			await _emailService.SendEmailAsync(context.Message.To, context.Message.Subject, context.Message.Body);
			Console.WriteLine("Email has been sent");
		}
	}
}
