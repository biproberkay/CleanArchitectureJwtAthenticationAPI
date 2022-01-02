using Application.Interfaces.Services;
using Application.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Shared.Communicate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SMTPMailService : IMailService
    {
        private readonly MailSettings _settings;
        private readonly ILogger<SMTPMailService> _logger;

        public SMTPMailService(IOptions<MailSettings> settings, ILogger<SMTPMailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendAsync(MailRequest request)
        {
            try
            {
                var email = new MimeMessage
                {
                    Sender = new MailboxAddress(_settings.DisplayName, request.From ?? _settings.From),
                    Subject = request.Subject,
                    Body = new BodyBuilder
                    {
                        HtmlBody = request.Body
                    }.ToMessageBody()
                };
                email.To.Add(MailboxAddress.Parse(request.To));
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_settings.UserName, _settings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            throw new NotImplementedException();
        }
    }
}
