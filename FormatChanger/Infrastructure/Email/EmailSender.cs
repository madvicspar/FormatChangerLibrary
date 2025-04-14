using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace FormatChanger.Infrastructure.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpHost = _configuration["SmtpSettings:Host"];
            var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
            var smtpUsername = _configuration["SmtpSettings:Username"];
            var smtpPassword = _configuration["SmtpSettings:Password"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUsername),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                try
                {
                    await client.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    // TODO: обработать включенный впн и прочее
                    return;
                }
            }
        }
    }
}
