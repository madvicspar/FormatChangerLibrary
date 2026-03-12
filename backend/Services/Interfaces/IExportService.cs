using FormatChanger.WebAPI.Models;
using FormatChanger.WebAPI.Models.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FormatChanger.WebAPI.Services.Interfaces
{
	public interface IExportService
	{
		Task<IActionResult> ExportAsync(DocumentModel document, ExportMethod method);
	}
}