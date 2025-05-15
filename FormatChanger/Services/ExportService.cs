using FormatChanger.Infrastructure.Email;
using FormatChanger.Models;
using FormatChanger.Models.Helpers;
using FormatChanger.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FormatChanger.Services
{
    public class ExportService : IExportService
    {
        private readonly IEmailSenderCustom _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExportService(IEmailSenderCustom emailSender, IHttpContextAccessor httpContextAccessor)
        {
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> ExportAsync(DocumentModel document, ExportMethod method)
        {
            if (document == null || string.IsNullOrWhiteSpace(document.FilePath) || !File.Exists(document.FilePath))
                return new NotFoundResult();

            return method switch
            {
                ExportMethod.Download => await ExportAsDownload(document),
                ExportMethod.Email => await ExportByEmail(document),
                ExportMethod.Telegram => await ExportToTelegram(document),
                _ => new BadRequestObjectResult("Неизвестный способ экспорта")
            };
        }

        private async Task<IActionResult> ExportAsDownload(DocumentModel document)
        {
            var fileBytes = await File.ReadAllBytesAsync(document.FilePath);
            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            var fileName = Path.GetFileName(document.FilePath);

            return new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = fileName
            };
        }

        private async Task<IActionResult> ExportByEmail(DocumentModel document)
        {
            var fileBytes = await File.ReadAllBytesAsync(document.FilePath);
            var fileName = Path.GetFileName(document.FilePath);

            var user = _httpContextAccessor.HttpContext?.User;
            var userEmail = user?.FindFirstValue(ClaimTypes.Email);
            var userName = user?.FindFirstValue(ClaimTypes.Name); // TODO: сохранять имя пользователя

            if (string.IsNullOrEmpty(userEmail))
            {
                return new BadRequestObjectResult("Не удалось определить email пользователя");
            }

            var recipientEmail = user?.FindFirstValue(ClaimTypes.Email);
            var subject = "Результаты работы format-changer"; // TODO: исправить текст письма
            var message = $@"
                <p>Здравствуйте, {userName ?? "уважаемый Пользователь"}!</p>
                <p>Во вложении находится документ <strong>{fileName}</strong>, в котором ..</p>
                <p>Если у вас есть вопросы, замечания или предложения, не стесняйтесь обращаться в ответ на это письмо</p>
                <p>С уважением,<br/>Команда FormatChanger</>";

            try
            {
                await _emailSender.SendEmailAsync(recipientEmail, subject, message, fileBytes, fileName);
                return new OkObjectResult("Документ отправлен по email");
            }
            catch (Exception ex)
            {
                return new ObjectResult("Не удалось отправить письмо: " + ex.Message)
                {
                    StatusCode = 500
                };
            }
        }

        private Task<IActionResult> ExportToTelegram(DocumentModel document)
        {
            // TODO: экспорт документа в Telegram
            throw new NotImplementedException();
        }
    }
}