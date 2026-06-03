namespace FormatChanger.Models
{
	public class ParagraphStyleProperties
	{
		public string StyleId { get; set; }
		public RunStyleProperties RunStyle { get; set; }

		public string SpacingLine { get; set; }
		public string SpacingBefore { get; set; }
		public string SpacingAfter { get; set; }

		public string IndentFirstLine { get; set; }
		public string IndentLeft { get; set; }
		public string IndentRight { get; set; }

		public string Justification { get; set; }
	}
}