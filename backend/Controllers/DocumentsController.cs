using FormatChanger.WebAPI.Models;
using FormatChanger.WebAPI.Models.Helpers;
using FormatChanger.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FormatChanger.WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DocumentsController : ControllerBase
	{
		private readonly IDocumentService _documentService;
		private readonly ITemplateService _templateService;
		private readonly IExportService _exportService;

		public DocumentsController(IDocumentService documentService, ITemplateService templateService, IExportService exportService)
		{
			_documentService = documentService;
			_templateService = templateService;
			_exportService = exportService;
		}

		[HttpPost("upload")]
		public async Task<IActionResult> Upload(IFormFile file)
		{
			if (file == null) return BadRequest("Файл не выбран или пуст.");

			var document = await _documentService.UploadDocumentAsync(file);
			var paragraphs = _documentService.ExtractParagraphs(document);
			return Ok(new { documentId = document.Id, paragraphs = paragraphs.Select(p => new { p.InnerText, p.Type }) });
		}

		[HttpPost("format")]
		public async Task<IActionResult> StartFormatting([FromQuery] long templateId, [FromQuery] int actionId, [FromQuery] long documentId, [FromBody] string[] types)
		{
			var document = await _documentService.GetDocumentByIdAsync(documentId);
			if (document == null)
			{
				return NotFound("Документ не найден.");
			}

			var template = await _templateService.GetTemplateByIdAsync(templateId);
			if (template == null)
				return NotFound("Шаблон не найден.");

			DocumentModel resultDocument;

			switch (actionId)
			{
				case 1: // Исправление
					resultDocument = await _documentService.CorrectDocumentAsync(document, template, types);
					break;
				case 2: // Проверка
					resultDocument = await _documentService.CheckDocumentAsync(document, template, types);
					break;
				case 3: // Оценивание
					resultDocument = await _documentService.EvaluateDocumentAsync(document, template, types);
					break;
				default:
					return BadRequest("Неизвестное действие");
			}

			return Ok(new { documentId = resultDocument.Id });
		}

		[HttpGet("export/{documentId}")]
		public async Task<IActionResult> Export(long documentId)
		{
			var document = await _documentService.GetDocumentByIdAsync(documentId);
			if (document == null)
				return NotFound("Документ не найден.");

			var result = await _exportService.ExportAsync(document, ExportMethod.Download);

			if (result is FileContentResult fileResult)
				return fileResult;

			if (result is ObjectResult objectResult)
				return Ok(new { status = objectResult.StatusCode, message = objectResult.Value });

			return Ok(new { status = 200, message = "Экспорт выполнен." });
		}
	}
}
