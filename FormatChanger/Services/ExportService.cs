using FormatChanger.Models;
using FormatChanger.Models.Helpers;
using FormatChanger.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FormatChanger.Services
{
    public class ExportService : IExportService
    {
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
            // TODO: экспорт документа по email
            throw new NotImplementedException();
        }

        private Task<IActionResult> ExportToTelegram(DocumentModel document)
        {
            // TODO: экспорт документа в Telegram
            throw new NotImplementedException();
        }
    }
}