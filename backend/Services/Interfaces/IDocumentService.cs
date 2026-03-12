using FormatChanger.WebAPI.Models;
using FormatChanger.WebAPI.Models.FormattingModels;
using FormatChanger.WebAPI.Models.Helpers;
namespace FormatChanger.WebAPI.Services.Interfaces
{
	public interface IDocumentService
	{
		// Метод для загрузки документа
		Task<DocumentModel> UploadDocumentAsync(IFormFile file);

		// Метод для получения документа по его ID
		Task<DocumentModel> GetDocumentByIdAsync(long id);

		// Метод для извлечения абзацев из документа
		List<ParagraphModel> ExtractParagraphs(DocumentModel document);
		// Метод для исправления форматирования документа
		Task<DocumentModel> CorrectDocumentAsync(DocumentModel document, FormattingTemplateModel template, string[] types);
		// Метод для проверки форматирования документа
		Task<DocumentModel> CheckDocumentAsync(DocumentModel document, FormattingTemplateModel template, string[] types);
		// Метод для оценивания форматирования документа
		Task<DocumentModel> EvaluateDocumentAsync(DocumentModel document, FormattingTemplateModel template, string[] types);
	}
}
