using FormatChanger.Models;
using FormatChanger.Utilities.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace FormatChanger.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;

        public DocumentService(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task FixDocumentFormattingAsync(DocumentModel document, int templateId)
        {
            
        }
    }
}