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

                var paragraphs = _documentService.GetDocumentParagraphs(document);

                SetTemplates();

                TempData["DocumentId"] = JsonConvert.SerializeObject(document.Id);

                return View("Index", paragraphs);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> StartFormattingProcess(long templateId, int actionId, [FromBody] string[] types)
        {
            var documentId = (long)JsonConvert.DeserializeObject<long>(TempData["DocumentId"].ToString());

            var document = await _documentService.GetDocumentByIdAsync(documentId);
            if (document == null)
            {
                return NotFound();
            }

            DocumentModel resultDocumentId;
            var template = _templateService.GetTemplateByIdAsync(templateId).Result;

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

            // Экспортируем документ
            return RedirectToAction("Index");
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
