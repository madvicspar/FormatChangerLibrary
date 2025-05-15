using FormatChanger.Models;
using FormatChanger.Models.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FormatChanger.Services.Interfaces
{
    public interface IExportService
    {
        Task<IActionResult> ExportAsync(DocumentModel document, ExportMethod method);
    }
}