using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;
using FormatChanger.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FormatChanger.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDocumentService _documentService;
        private readonly ITemplateService _templateService;

        public HomeController(ILogger<HomeController> logger, IDocumentService documentService, ITemplateService templateService)
        {
            _logger = logger;
            _documentService = documentService;
            _templateService = templateService;
        }

        public IActionResult Index(IEnumerable<Paragraph> paragraphs = null)
        {
            var templates = _templateService.GetTemplatesAsync();

            ViewBag.Templates = templates.Result;

            return View(paragraphs);
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

                var paragraphs = _documentService.GetDocumentParagraphs(document);

                return View("Index", paragraphs);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Export(long templateId, int actionId)
        {
            // получить шаблон
            // если оценивание, то получить систему оценивания
            // если проверка, то тип исправлений
            // что-то делаем в зависимости от типа



            // Извлекаем документ и шаблон из TempData
            var documentId = TempData["DocumentId"] as int?;
            if (!documentId.HasValue)
            {
                return BadRequest("Документ не найден.");
            }

            var document = await _documentService.GetDocumentByIdAsync(documentId.Value);
            if (document == null)
            {
                return NotFound();
            }

            DocumentModel resultDocumentId;

            switch (actionId)
            {
                case 1: // Исправление
                    resultDocumentId = await _documentService.CorrectDocumentAsync(document, templateId);
                    break;
                case 2: // Проверка
                    resultDocumentId = await _documentService.CheckDocumentAsync(document, templateId);
                    break;
                case 3: // Оценивание
                    resultDocumentId = await _documentService.EvaluateDocumentAsync(document, templateId);
                    break;
                default:
                    return BadRequest("Неизвестное действие");
            }

            // Экспортируем документ
            return View();
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
    }
}
