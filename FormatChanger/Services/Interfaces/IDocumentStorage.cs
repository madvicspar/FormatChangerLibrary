using FormatChanger.Models;

namespace FormatChanger.Services.Interfaces
{
    public interface IDocumentStorage
    {
        Task<DocumentModel> SaveAsync(IFormFile file);
        Task<DocumentModel> GetByIdAsync(long id);
    }
}