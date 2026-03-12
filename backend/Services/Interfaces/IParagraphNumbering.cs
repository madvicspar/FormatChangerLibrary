using DocumentFormat.OpenXml.Packaging;

using FormatChanger.WebAPI.Models.Helpers;
namespace FormatChanger.WebAPI.Services.Interfaces
{
	public interface IParagraphNumbering
	{
		void Apply(WordprocessingDocument doc, List<ParagraphModel> paragraphs);
	}
}