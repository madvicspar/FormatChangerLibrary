using DocumentFormat.OpenXml.Packaging;

using FormatChanger.WebAPI.Models.FormattingModels;
using FormatChanger.WebAPI.Models.Helpers;

namespace FormatChanger.WebAPI.Services.Interfaces
{
	public interface IDocumentChecker
	{
		Task CheckAndCommentAsync(WordprocessingDocument doc, FormattingTemplateModel template, List<ParagraphModel> paragraphs, string[] types);
	}
}