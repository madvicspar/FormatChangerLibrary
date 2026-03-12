using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using FormatChanger.WebAPI.Extensions;
using FormatChanger.WebAPI.Models.Helpers;
using FormatChanger.WebAPI.Services.Interfaces;

namespace FormatChanger.WebAPI.Services
{
	public class ParagraphStyler : IParagraphStyler
	{
		public void ApplyStyles(WordprocessingDocument doc, List<ParagraphModel> paragraphList, string[] types)
		{
			var styles = doc.MainDocumentPart.StyleDefinitionsPart.Styles;
			var paragraphs = doc.MainDocumentPart.Document.Body.Descendants<Paragraph>().ToList();

			for (int i = 0; i < types.Length; i++)
			{
				paragraphList[i].Type = ParagraphTypeExtensions.ToEnum(types[i]).ToString();
			}

			foreach (var p in paragraphs)
			{
				var match = paragraphList.FirstOrDefault(x => x.Paragraph.ParagraphId == p.ParagraphId);
				if (match == null) continue;

				var props = p.Elements<ParagraphProperties>().FirstOrDefault() ?? new ParagraphProperties();
				var type = match.Type switch
				{
					var t when t == ParagraphTypes.FirstH.ToString() => "heading 1",
					var t when t == ParagraphTypes.SecondH.ToString() => "heading 2",
					var t when t == ParagraphTypes.ThirdH.ToString() => "heading 3",
					_ when IsList(match.Type) => "Normal",
					_ => match.Type
				};

				var styleId = styles.Elements<Style>().FirstOrDefault(s => s.StyleName?.Val == type)?.StyleId;
				if (styleId != null)
				{
					props.ParagraphStyleId = new ParagraphStyleId { Val = styleId };
				}
			}
			doc.Save();
		}
		public void EnsureStylesExists(Styles styles, ParagraphTypes type)
		{
			if (styles.Elements<Style>().All(s => s.StyleName?.Val != type.ToString()))
			{
				var style = new Style()
				{
					Type = StyleValues.Paragraph,
					StyleId = type.ToString(),
					CustomStyle = true,
					StyleName = new StyleName() { Val = type.ToString() }
				};

				style.Append(new BasedOn() { Val = "Normal" });
				style.Append(new NextParagraphStyle() { Val = "Normal" });
				styles.Append(style);
			}
		}
		public void CleanFormat(WordprocessingDocument doc)
		{
			var body = doc.MainDocumentPart.Document.Body;
			foreach (var paragraph in body.Elements<Paragraph>())
			{
				var paragraphProperties = paragraph.Elements<ParagraphProperties>().FirstOrDefault();
				if (paragraphProperties != null)
				{
					var styleElement = paragraphProperties.Elements<ParagraphStyleId>().FirstOrDefault();
					var numberingProperties = paragraphProperties.Elements<NumberingProperties>().FirstOrDefault();
					paragraphProperties.RemoveAllChildren();

					if (styleElement != null)
					{
						paragraphProperties.Append(styleElement);
					}
					if (numberingProperties != null)
					{
						paragraphProperties.Append(numberingProperties);
					}
				}

				foreach (var run in paragraph.Elements<Run>())
				{
					var runProperties = run.Elements<RunProperties>().FirstOrDefault();
					if (runProperties != null)
					{
						var styleElement = runProperties.Elements<RunStyle>().FirstOrDefault();
						runProperties.RemoveAllChildren();
						if (styleElement != null)
						{
							runProperties.Append(styleElement);
						}
					}
				}
			}
		}
		private bool IsList(string t) =>
		t == ParagraphTypes.Dash.ToString() || t == ParagraphTypes.Period.ToString() || t == ParagraphTypes.Bracket.ToString();
	}
}