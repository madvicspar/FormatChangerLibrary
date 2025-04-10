using FormatChanger.Models;
using FormatChanger.Utilities.Data;

namespace FormatChanger.Services
{
    public class DocumentStorageService : IDocumentStorage
    {
        private readonly ApplicationDbContext _context;
        public DocumentStorageService(ApplicationDbContext context) => _context = context;
        public async Task<DocumentModel> SaveAsync(IFormFile file)
        {
            var user = _context.Users.First();
            var doc = new DocumentModel
            {
                FilePath = Path.Combine("uploads", file.FileName),
                IsOriginal = true,
                UserId = user.Id.ToString()
            };

            using var stream = new FileStream(doc.FilePath, FileMode.Create);
            await file.CopyToAsync(stream);

            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();
            return doc;
        }
        public async Task<DocumentModel> GetByIdAsync(long id) => await _context.Documents.FindAsync(id);
    }
}