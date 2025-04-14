using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models.Helpers;
using FormatChanger.Services.Interfaces;

namespace FormatChanger.Services
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
            foreach (var paragraph in paragraphs)
            {
                var type = paragraphList.Where(x => x.Paragraph.ParagraphId == paragraph.ParagraphId).First().Type;
                if (!IsList(type))
                    continue;

                var paraProps = paragraph.Elements<ParagraphProperties>().FirstOrDefault() ?? new ParagraphProperties();

                var level = DetermineListLevel(paragraphList, levels, type, paragraph);
                var numberingId = type switch
                {
                    var t when t == ParagraphTypes.Dash.ToString() => 1001,
                    var t when t == ParagraphTypes.Period.ToString() => 1002,
                    var t when t == ParagraphTypes.Bracket.ToString() => 1003,
                    _ => 1001
                };
                ApplyNumbering(paraProps, level, numberingId);
            }
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
        private int DetermineListLevel(List<ParagraphModel> paragraphList, Stack<int> stack, string type, Paragraph paragraph)
        {
            int level = 0;
            var index = paragraphList.FindIndex(p => p.Paragraph.ParagraphId == paragraph.ParagraphId);
            var prev = index > 0 ? paragraphList[index - 1] : null;
            if (stack.Count > 0 && prev != null && IsList(prev.Type))
            {
                level = prev.Type != type ? stack.Peek() + 1 : stack.Peek();
            }
            stack.Push(level);
            return level;
        }
        private void ApplyNumbering(ParagraphProperties paraProps, int level, int id)
        {
            var numberingProperties = new NumberingProperties(
                new NumberingLevelReference { Val = level },
                new NumberingId { Val = id }
            );

            paraProps.Append(numberingProperties);
            var indentation = paraProps.Elements<Indentation>().FirstOrDefault() ?? new Indentation();

            // Слева: 1.5 * (level + 1), в EMU
            indentation.Left = ((int)((2 + 0.5 * level) * 567)).ToString();
            indentation.Hanging = ((int)(0.5 * 567)).ToString();
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
            numbering.Append(CreateAbstractNum(1002, "1.", NumberFormatValues.Decimal));
            numbering.Append(CreateAbstractNum(1003, "1)", NumberFormatValues.Decimal));

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