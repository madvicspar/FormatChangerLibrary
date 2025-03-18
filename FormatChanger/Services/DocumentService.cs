using FormatChanger.Models;
using FormatChanger.Utilities.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System.Xml.Linq;

namespace FormatChanger.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IElementCorrectionStrategy<TextSettingsModel> _textCorrectionStrategy;
        private readonly IElementCorrectionStrategy<HeadingSettingsModel> _headingFirstCorrectionStrategies;
        private readonly IElementCorrectionStrategy<ImageSettingsModel> _imageCorrectionStrategy;
        private readonly IElementCorrectionStrategy<TableSettingsModel> _tableCorrectionStrategy;
        private readonly IElementCorrectionStrategy<CellSettingsModel> _cellCorrectionStrategy;

        public DocumentService(ApplicationDbContext context, 
            IElementCorrectionStrategy<TextSettingsModel> textStrategy,
            IElementCorrectionStrategy<HeadingSettingsModel> h1Strategy,
            IElementCorrectionStrategy<ImageSettingsModel> imageStrategy,
            IElementCorrectionStrategy<TableSettingsModel> tableCorrectionStrategy,
            IElementCorrectionStrategy<CellSettingsModel> cellCorrectionStrategy)
        {
            _context = context;
            _textCorrectionStrategy = textStrategy;
            _headingFirstCorrectionStrategies = h1Strategy;
            _imageCorrectionStrategy = imageStrategy;
            _tableCorrectionStrategy = tableCorrectionStrategy;
            _cellCorrectionStrategy = cellCorrectionStrategy;
            //_context.SeedData(_context);
        }

        // Загрузка документа и сохранение в БД
        public async Task<DocumentModel> UploadDocumentAsync(IFormFile file)
        {
            var users = _context.Users.First();
            var document = new DocumentModel
            {
                FilePath = Path.Combine("uploads", file.FileName),
                IsOriginal = true,
                UserId = _context.Users.First().Id.ToString()
            };

            // Сохраняем файл на сервер
            using (var stream = new FileStream(document.FilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return document;
        }

        // Получаем документ по его ID
        public async Task<DocumentModel> GetDocumentByIdAsync(long id)
        {
            return await _context.Documents.FindAsync(id);
        }

        // Извлечение абзацев из документа
        public List<Paragraph> GetDocumentParagraphs(DocumentModel document)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(document.FilePath, true))
            {
                var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().ToList();
                return paragraphs;
            }
        }

        public async Task<DocumentModel> CorrectDocumentAsync(DocumentModel document, FormattingTemplateModel template)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(document.FilePath, true))
            {

                _textCorrectionStrategy.ApplyCorrection(doc, template);
                _headingFirstCorrectionStrategies.ApplyCorrection(doc, template);
                _imageCorrectionStrategy.ApplyCorrection(doc, template);
                _tableCorrectionStrategy.ApplyCorrection(doc, template);
                _cellCorrectionStrategy.ApplyCorrection(doc, template);
                
                doc.Save();
            }
            // TODO: достать исправленный документ
            return document;
        }

        public async Task<DocumentModel> CheckDocumentAsync(DocumentModel document, FormattingTemplateModel template)
        {
            throw new NotImplementedException();
        }

        public async Task<DocumentModel> EvaluateDocumentAsync(DocumentModel document, FormattingTemplateModel template)
        {
            throw new NotImplementedException();
        }
    }
}