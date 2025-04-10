using DocumentFormat.OpenXml.Packaging;
using FormatChanger.Models;
using FormatChanger.Services.Interfaces;

namespace FormatChanger.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentStorage _storage;
        private readonly IParagraphExtractor _extractor;
        private readonly IDocumentCorrector _corrector;
        private readonly IDocumentChecker _checker;
        private readonly IParagraphStyler _styler;
        private readonly IParagraphNumbering _numbering;
        public DocumentService(IDocumentStorage storage,
            IParagraphExtractor extractor,
            IDocumentCorrector corrector,
            IDocumentChecker checker,
            IParagraphStyler styler,
            IParagraphNumbering numbering)
        {
            _storage = storage;
            _extractor = extractor;
            _corrector = corrector;
            _checker = checker;
            _styler = styler;
            _numbering = numbering;
        }

        public async Task<DocumentModel> UploadDocumentAsync(IFormFile file) =>
            await _storage.SaveAsync(file);

        public async Task<DocumentModel> GetDocumentByIdAsync(long id) =>
            await _storage.GetByIdAsync(id);

        public List<ParagraphModel>? ExtractParagraphs(DocumentModel document)
        {
            using var doc = WordprocessingDocument.Open(document.FilePath, false);
            return _extractor.Extract(doc);
        }

        public async Task<DocumentModel> CorrectDocumentAsync(DocumentModel document, FormattingTemplateModel template, string[] types)
        {
            using var doc = WordprocessingDocument.Open(document.FilePath, true);
            var styles = doc.MainDocumentPart.StyleDefinitionsPart.Styles;
            var paragraphs = _extractor.Extract(doc);

            _styler.EnsureStylesExists(styles, ParagraphTypes.ImageCaption);
            _styler.EnsureStylesExists(styles, ParagraphTypes.TableCaption);
            styles.Save();

            _styler.ApplyStyles(doc, paragraphs, types);

            _styler.CleanFormat(doc);
            await _corrector.ApplyAllStrategiesAsync(doc, template, paragraphs);
            _numbering.Apply(doc, paragraphs);

            doc.Save();
            return document;
        }

        public async Task<DocumentModel> CheckDocumentAsync(DocumentModel document, FormattingTemplateModel template, string[] types)
        {
            using var doc = WordprocessingDocument.Open(document.FilePath, true);
            var paragraphs = _extractor.Extract(doc);
            await _checker.CheckAndCommentAsync(doc, template, paragraphs, types);
            return document;
        }

        public async Task<DocumentModel> EvaluateDocumentAsync(DocumentModel document, FormattingTemplateModel template, string[] types)
        {
            throw new NotImplementedException();
        }
    }
}