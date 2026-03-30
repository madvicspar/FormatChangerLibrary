using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using FormatChanger.WebAPI.Extensions;
using FormatChanger.WebAPI.Models.Helpers;
using FormatChanger.WebAPI.Services.Interfaces;

namespace FormatChanger.WebAPI.Services
{
	public class ParagraphNumbering : IParagraphNumbering
	{
		public void Apply(WordprocessingDocument doc, List<ParagraphModel> paragraphList)
		{
			AddPageNumbering(doc);
			AddNumberingToDocument(doc);

			var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>()
			.Where(p => !string.IsNullOrWhiteSpace(p.InnerText) && !p.Ancestors<TableCell>().Any())
			.ToList();

			Stack<int> levels = new();
			Stack<ParagraphTypes> markers = new();
			foreach (var paragraph in paragraphs)
			{
				var type = paragraphList.Where(x => x.Paragraph.ParagraphId == paragraph.ParagraphId).First().Type;
				if (!IsList(type))
					continue;

				var paraProps = paragraph.Elements<ParagraphProperties>().FirstOrDefault() ?? new ParagraphProperties();

				var level = DetermineListLevel(paragraphList, levels, markers, type, paragraph);
				var numberingId = type switch
				{
					var t when t == ParagraphTypes.Dash.ToString() => 1001,
					var t when t == ParagraphTypes.Period.ToString() => 1002,
					var t when t == ParagraphTypes.Bracket.ToString() => 1003,
					_ => 1001
				};
				ApplyNumbering(paraProps, level, numberingId);
				doc.Save();
			}
			doc.Save();
		}
		public void AddPageNumbering(WordprocessingDocument doc)
		{
			// TODO: привязать к обычному тексту?
			FooterPart footerPart = doc.MainDocumentPart.AddNewPart<FooterPart>();
			string footerPartId = doc.MainDocumentPart.GetIdOfPart(footerPart);

			Footer footer = new(new Paragraph(
				new ParagraphProperties(
					new ParagraphStyleId() { Val = "Normal" },
					new Justification() { Val = JustificationValues.Center },
					new SpacingBetweenLines() { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto },
					new Indentation { Left = "0", Right = "0", FirstLine = "0" }
				),
				new Run(
					new SimpleField() { Instruction = "PAGE" })));

			footerPart.Footer = footer;

			IEnumerable<SectionProperties> sectionProperties = doc.MainDocumentPart.Document.Body.Elements<SectionProperties>();

			foreach (var sectionProperty in sectionProperties)
			{
				sectionProperty.RemoveAllChildren<FooterReference>();
				sectionProperty.PrependChild(new FooterReference()
				{
					Id = footerPartId
				});
			}
		}
		private int DetermineListLevel(List<ParagraphModel> paragraphList, Stack<int> levels, Stack<ParagraphTypes> markers, string type, Paragraph paragraph)
		{
			int level = 0;
			var index = paragraphList.FindIndex(p => p.Paragraph.ParagraphId == paragraph.ParagraphId);
			var prev = index > 0 ? paragraphList[index - 1] : null;
			if (levels.Count > 0 && prev != null && IsList(prev.Type))
			{
				if (prev.Type == type)
				{
					level = levels.Peek();
				}
				else if (markers.Count > 1 && markers.ElementAtOrDefault(markers.Count - 2).ToString() == type)
				{
					level = levels.Pop() - 1;
					markers.Pop();
				}
				else
				{
					level = levels.Peek() + 1;
					levels.Push(level);
					markers.Push(type.ToEnum());
				}
			}
			else
			{
				levels.Push(level);
				markers.Push(type.ToEnum());
			}
			return level;
		}
		private void ApplyNumbering(ParagraphProperties paraProps, int level, int id)
		{
			var numberingProperties = new NumberingProperties(
				new NumberingLevelReference { Val = level },
				new NumberingId { Val = id }
			);

			paraProps.Append(numberingProperties);
			paraProps.RemoveAllChildren<Indentation>();

			// Слева: 1.5 * (level + 1), в EMU
			var indentation = new Indentation();
			indentation.Left = ((int)((2 + 0.5 * level) * 567)).ToString();
			indentation.Hanging = ((int)(0.5 * 567)).ToString();

			paraProps.AppendChild(indentation);
		}
		public bool IsList(string type)
		{
			return type == ParagraphTypes.Period.ToString()
				|| type == ParagraphTypes.Bracket.ToString()
				|| type == ParagraphTypes.Dash.ToString();
		}
		public void AddNumberingToDocument(WordprocessingDocument doc)
		{
			var numberingPart = doc.MainDocumentPart.NumberingDefinitionsPart
				?? doc.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>();

			if (numberingPart.Numbering == null)
			{
				numberingPart.Numbering = new Numbering();
			}

			var numbering = numberingPart.Numbering;

			numbering.Append(CreateAbstractNum(1001, "-", NumberFormatValues.Bullet));
			numbering.Append(CreateAbstractNum(1002, "%1.", NumberFormatValues.Decimal));
			numbering.Append(CreateAbstractNum(1003, "%1)", NumberFormatValues.Decimal));

			numbering.Append(new NumberingInstance(new AbstractNumId { Val = 1001 }) { NumberID = 1001 });
			numbering.Append(new NumberingInstance(new AbstractNumId { Val = 1002 }) { NumberID = 1002 });
			numbering.Append(new NumberingInstance(new AbstractNumId { Val = 1003 }) { NumberID = 1003 });

			numberingPart.Numbering.Save();
		}
		public AbstractNum CreateAbstractNum(int id, string marker, NumberFormatValues format)
		{
			var num = new AbstractNum { AbstractNumberId = id };
			for (int i = 0; i < 3; i++)
			{
				num.AppendChild(new Level(
					new StartNumberingValue { Val = 1 },
					new NumberingFormat { Val = format },
					new LevelText { Val = marker },
					new LevelJustification { Val = LevelJustificationValues.Left },
					new LevelSuffix { Val = LevelSuffixValues.Space }
				)
				{ LevelIndex = i });
			}
			return num;
		}
	}
}