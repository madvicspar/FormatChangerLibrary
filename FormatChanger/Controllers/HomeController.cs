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
            // TODO: ������ �� ����� �������, ���� �� �� ������ ����, ����� ���� �������� ������ �������������
            var templates = _templateService.GetTemplatesAsync();
            ViewBag.Templates = templates.Result;
        }

        private const long MaxFileSizeBytes = 20 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".docx" };

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please select a file.");
                SetTemplates();
                return View("Index");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("file", "Only .docx files are supported.");
                SetTemplates();
                return View("Index");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                ModelState.AddModelError("file", "File size must not exceed 20 MB.");
                SetTemplates();
                return View("Index");
            }

            _logger.LogInformation("Uploading document: {FileName} ({Size} bytes)", file.FileName, file.Length);

            var document = await _documentService.UploadDocumentAsync(file);
            var paragraphs = _documentService.ExtractParagraphs(document);

            HttpContext.Session.SetString("DocumentId", document.Id.ToString());

            SetTemplates();
            return View("Index", paragraphs);
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

            if (actionId == 3)
                return StatusCode(501, "Document evaluation is not yet available.");

            var template = await _templateService.GetTemplateByIdAsync(templateId);
            if (template == null)
                return NotFound("Template not found.");

            if (template.HeadingSettings != null)
                template.HeadingLevelsEdit = FlattenHeadings(template.HeadingSettings);

            switch (actionId)
            {
                case 1:
                    await _documentService.CorrectDocumentAsync(document, template, types);
                    break;
                case 2:
                    await _documentService.CheckDocumentAsync(document, template, types);
                    break;
                default:
                    return BadRequest("Unknown action.");
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
                message = "������� ��������"
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

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> SaveTemplate(FormattingTemplateModel model)
		{
			if (!ModelState.IsValid)
				return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
			var id = await _templateService.SaveTemplateAsync(model);
			return Json(new { success = true, id });
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteTemplate(long id)
		{
			await _templateService.DeleteTemplateAsync(id);
			return Json(new { success = true });
		}

		[HttpGet]
		public async Task<IActionResult> GetTemplateForEdit(long id)
		{
			FormattingTemplateModel model;
			if (id == 0)
			{
				model = CreateDefaultModel();
			}
			else
			{
				model = await _templateService.GetTemplateByIdAsync(id);
				if (model == null) return NotFound();
				if (model.HeadingSettings != null)
					model.HeadingLevelsEdit = FlattenHeadings(model.HeadingSettings);
			}
			return PartialView("_FormattingTemplateForm", model);
		}

		private static FormattingTemplateModel CreateDefaultModel() => new()
		{
			DocumentSettings = new DocumentSettingsModel(),
			TextSettings = new TextSettingsModel(),
			ListSettings = new ListSettingsModel
			{
				TextSettings = new TextSettingsModel(),
				BulletListSettings = new BulletListSettingsModel(),
				NumberedListSettings = new NumberedListSettingsModel()
			},
			TableSettings = new TableSettingsModel
			{
				CaptionSettings = new TableCaptionSettingsModel { TextSettings = new TextSettingsModel() },
				CellSettings = new CellSettingsModel { TextSettings = new TextSettingsModel() },
				HeaderSettings = new HeaderSettingsModel
				{
					CellSettings = new CellSettingsModel { TextSettings = new TextSettingsModel() }
				}
			},
			ImageSettings = new ImageSettingsModel
			{
				CaptionSettings = new ImageCaptionSettingsModel { TextSettings = new TextSettingsModel() }
			}
		};

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
