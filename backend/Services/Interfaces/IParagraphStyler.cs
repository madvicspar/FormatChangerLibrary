using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using FormatChanger.WebAPI.Models.Helpers;
namespace FormatChanger.WebAPI.Services.Interfaces
{
	public interface IParagraphStyler
	{
		void ApplyStyles(WordprocessingDocument doc, List<ParagraphModel> paragraphs, string[] types);
		void EnsureStylesExists(Styles styles, ParagraphTypes type);
		void CleanFormat(WordprocessingDocument doc);
	}
}