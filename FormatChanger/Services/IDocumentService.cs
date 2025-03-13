using FormatChanger.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Wordprocessing;

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
    }
}
