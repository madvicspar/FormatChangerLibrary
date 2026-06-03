using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace FormatChanger.Infrastructure.Email
{
    public class EmailSender : IEmailSender, IEmailSenderCustom
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
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
                    _logger.LogError(ex, "Failed to send email to {Recipient} with subject '{Subject}'", email, subject);
                    return;
                }
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage, byte[] fileBytes, string fileName)
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

                if (fileBytes != null)
                {
                    var stream = new MemoryStream(fileBytes, false);
                    var attachment = new Attachment(stream, fileName);
                    mailMessage.Attachments.Add(attachment);
                }

                try
                {
                    await client.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email with attachment to {Recipient} with subject '{Subject}'", email, subject);
                    return;
                }
                finally
                {
                    foreach (var attachment in mailMessage.Attachments)
                    {
                        attachment.Dispose();
                    }
                }
            }
        }
    }
}