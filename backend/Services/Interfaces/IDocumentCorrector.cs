using DocumentFormat.OpenXml.Packaging;

using FormatChanger.WebAPI.Models.FormattingModels;
using FormatChanger.WebAPI.Models.Helpers;

namespace FormatChanger.WebAPI.Services.Interfaces
{
	public interface IDocumentCorrector
	{
		Task ApplyAllStrategiesAsync(WordprocessingDocument doc, FormattingTemplateModel template, List<ParagraphModel> paragraphs);
	}
}