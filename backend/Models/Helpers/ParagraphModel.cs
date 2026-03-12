using DocumentFormat.OpenXml.Wordprocessing;

namespace FormatChanger.WebAPI.Models.Helpers
{
	public class ParagraphModel
	{
		public Paragraph Paragraph { get; set; }
		public string Type { get; set; }
		public string InnerText => Paragraph.InnerText;
	}
}
