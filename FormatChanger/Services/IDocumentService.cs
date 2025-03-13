using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public interface IDocumentService
    {
        // Метод для загрузки документа
        Task<DocumentModel> UploadDocumentAsync(IFormFile file);

        // Метод для получения документа по его ID
        Task<DocumentModel> GetDocumentByIdAsync(long id);

        // Метод для извлечения абзацев из документа
        List<Paragraph> GetDocumentParagraphs(DocumentModel document);
        // Метод для исправления форматирования документа
        Task<DocumentModel> CorrectDocumentAsync(DocumentModel document, long templateId);
        // Метод для проверки форматирования документа
        Task<DocumentModel> CheckDocumentAsync(DocumentModel document, long templateId);
        // Метод для оценивания форматирования документа
        Task<DocumentModel> EvaluateDocumentAsync(DocumentModel document, long templateId);
    }
}
