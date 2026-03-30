using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services.Interfaces
{
	public interface IDocumentCommand
    {
		Task<DocumentModel> ExecuteAsync(DocumentModel document, FormattingTemplateModel template, string[] types);
	}
}