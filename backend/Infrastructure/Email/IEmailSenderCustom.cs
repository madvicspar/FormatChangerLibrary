using Microsoft.AspNetCore.Identity.UI.Services;

namespace FormatChanger.WebAPI.Infrastructure.Email
{
	public interface IEmailSenderCustom : IEmailSender
	{
		Task SendEmailAsync(string email, string subject, string htmlMessage, byte[] fileBytes, string fileName);
	}
}