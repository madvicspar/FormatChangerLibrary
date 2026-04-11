using System.Diagnostics;

using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;
using FormatChanger.Models.Helpers;
using FormatChanger.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormatChanger.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDocumentService _documentService;
        private readonly ITemplateService _templateService;
        private readonly IExportService _exportService;

        public HomeController(ILogger<HomeController> logger, IDocumentService documentService, ITemplateService templateService, IExportService exportService)
        {
            _logger = logger;
            _documentService = documentService;
            _templateService = templateService;
            _exportService = exportService;
        }

        public IActionResult Index(List<ParagraphModel> paragraphs = null)
        {
            SetTemplates();
            return View(paragraphs);
        }

        public void SetTemplates()
        {
            // TODO: убрать из типов подпись, если ее не должно быть, после чего поменять логику классификации
            var templates = _templateService.GetTemplatesAsync();
            ViewBag.Templates = templates.Result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null)
            {
                var document = await _documentService.UploadDocumentAsync(file);
                var _document = await _documentService.GetDocumentByIdAsync(document.Id);
                if (_document == null)
                {
                    return NotFound();
                }

                var paragraphs = _documentService.ExtractParagraphs(document);

                SetTemplates();

                HttpContext.Session.SetString("DocumentId", document.Id.ToString());

                return View("Index", paragraphs);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> StartFormattingProcess(long templateId, int actionId, [FromBody] string[] types)
        {
            var documentIdStr = HttpContext.Session.GetString("DocumentId");
            if (!long.TryParse(documentIdStr, out var documentId))
                return BadRequest();

            var document = await _documentService.GetDocumentByIdAsync(documentId);
            if (document == null)
            {
                return NotFound();
            }

            DocumentModel resultDocumentId;
            var template = _templateService.GetTemplateByIdAsync(templateId).Result;

			if (template.HeadingSettings != null)
			{
				template.HeadingLevelsEdit = FlattenHeadings(template.HeadingSettings);
			}

			switch (actionId)
            {
                case 1: // Исправление
                    resultDocumentId = await _documentService.CorrectDocumentAsync(document, template, types);
                    break;
                case 2: // Проверка
                    resultDocumentId = await _documentService.CheckDocumentAsync(document, template, types);
                    break;
                case 3: // Оценивание
                    resultDocumentId = await _documentService.EvaluateDocumentAsync(document, template, types);
                    break;
                default:
                    return BadRequest("Неизвестное действие");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            var documentIdStr = HttpContext.Session.GetString("DocumentId");
            if (!long.TryParse(documentIdStr, out var documentId))
                return BadRequest();

            var document = await _documentService.GetDocumentByIdAsync(documentId);

            var result = await _exportService.ExportAsync(document, ExportMethod.Download);
            if (result is FileContentResult)
                return result;

            if (result is ObjectResult objectResult)
            {
                return Json(new
                {
                    status = objectResult.StatusCode,
                    message = objectResult.Value
                });
            }

            return Json(new
            {
                status = 200,
                message = "Экспорт завершен"
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

		private List<HeadingLevelEditItem> FlattenHeadings(HeadingSettingsModel head)
		{
			var list = new List<HeadingLevelEditItem>();
			var current = head;
			while (current != null)
			{
				list.Add(new HeadingLevelEditItem
				{
					Id = current.Id,
					HeadingLevel = current.HeadingLevel,
					StartOnNewPage = current.StartOnNewPage,
					TextSettings = current.TextSettings ?? new TextSettingsModel()
				});
				current = current.NextHeadingLevel;
			}
			return list;
		}

		private HeadingSettingsModel BuildHeadingChain(List<HeadingLevelEditItem> items)
		{
			if (items == null || items.Count == 0) return null;

			HeadingSettingsModel first = null;
			HeadingSettingsModel prev = null;

			foreach (var item in items.OrderBy(i => i.HeadingLevel))
			{
				var heading = new HeadingSettingsModel
				{
					HeadingLevel = item.HeadingLevel,
					StartOnNewPage = item.StartOnNewPage,
					TextSettings = item.TextSettings
				};
				if (first == null) first = heading;
				if (prev != null) prev.NextHeadingLevel = heading;
				prev = heading;
			}
			return first;
		}
	}
}
