using System.Security.Claims;
using FormatChanger.Models;
using FormatChanger.Services.Interfaces;
using FormatChanger.Utilities.Data;
using Microsoft.AspNetCore.Http;

namespace FormatChanger.Services
{
    public class DocumentStorageService : IDocumentStorage
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DocumentStorageService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DocumentModel> SaveAsync(IFormFile file)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            var doc = new DocumentModel
            {
                FilePath = Path.Combine("uploads", file.FileName),
                IsOriginal = true,
                UserId = userId ?? string.Empty
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