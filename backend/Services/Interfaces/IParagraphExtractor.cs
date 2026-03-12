using DocumentFormat.OpenXml.Packaging;

using FormatChanger.WebAPI.Models.Helpers;
namespace FormatChanger.WebAPI.Services.Interfaces
{
	public interface IParagraphExtractor
	{
		List<ParagraphModel>? Extract(WordprocessingDocument doc);
	}
}