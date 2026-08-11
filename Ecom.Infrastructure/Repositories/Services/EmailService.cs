using Ecom.Core.DTO;
using Ecom.Core.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task sendEmail(EmailDTO emailDTO)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("My Ecom",_configuration["EmailSetings:From"]));
            message.Subject = emailDTO.Subject;
            message.To.Add(new MailboxAddress(emailDTO.To, emailDTO.To));
            message.Body =  new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailDTO.Content
            };
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(
                        _configuration["EmailSetings:smtp"],
                        Convert.ToInt32(_configuration["EmailSetings:Port"]), true);

                    await smtp.AuthenticateAsync(
                        _configuration["EmailSetings:Username"],
                        _configuration["EmailSetings:Password"]);

                    await smtp.SendAsync(message);
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    smtp.Disconnect(true);
                    smtp.Dispose();
                }
            }
        }
    }
}
