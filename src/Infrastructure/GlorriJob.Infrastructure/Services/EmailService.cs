using GlorriJob.Application.Abstractions.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
namespace GlorriJob.Infrastructure.Services
{
	public class EmailService : IEmailService
	{
		private string _smtpServer { get; }
		private int _smtpPort { get; }
		private string _smtpUsername { get; }
		private string _smtpPassword { get; }

		public EmailService(IConfiguration configuration)
		{
			_smtpServer = configuration["EmailSettings:SmtpServer"] ?? string.Empty;
			_smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? string.Empty);
			_smtpUsername = configuration["EmailSettings:SmtpUsername"] ?? string.Empty;
			_smtpPassword = configuration["EmailSettings:SmtpPassword"] ?? string.Empty;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string body)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("GlorriJob", "glorrijob@example.com"));
			message.To.Add(new MailboxAddress("", toEmail));
			message.Subject = subject;

			message.Body = new TextPart("plain")
			{
				Text = body
			};

			using (var client = new SmtpClient())
			{
				try
				{
					await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
					await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
					await client.SendAsync(message);
					await client.DisconnectAsync(true);
					Console.WriteLine("Email sent successfully!");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error sending email: {ex.Message}");
				}
			}
		}
	}
}
