using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models.Helpers;
using FormatChanger.Services.Interfaces;

namespace FormatChanger.Services
{
    public class ParagraphExtractor : IParagraphExtractor
    {
        public List<ParagraphModel>? Extract(WordprocessingDocument doc)
        {
            var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().ToList();
            if (paragraphs == null)
                return null;

            var styles = doc.MainDocumentPart.StyleDefinitionsPart.Styles;
            var listContext = new ListContext();

            return paragraphs
                .Select((p, i) =>
                    new ParagraphModel
                    {
                        Paragraph = p,
                        Type = DetermineParagraphType(doc, p, paragraphs, i, styles, listContext)
                    })
                .Where(p => !string.IsNullOrWhiteSpace(p.InnerText) && !p.Paragraph.Ancestors<TableCell>().Any())
                .ToList();
        }

        private static string DetermineParagraphType(
            WordprocessingDocument doc,
            Paragraph paragraph,
            IReadOnlyList<Paragraph> allParagraphs,
            int index,
            Styles? styles,
            ListContext listContext)
        {
            var styleName = GetParagraphTypeFromStyle(paragraph, allParagraphs, index, styles);

            if (!string.IsNullOrEmpty(styleName) && styleName != ParagraphTypes.Normal.ToString())
                return styleName;

            var marker = GetListMarker(doc, paragraph);

            if (string.IsNullOrEmpty(marker))
            {
                listContext.Reset();
                return ParagraphTypes.Normal.ToString();
            }

            return GetListParagraphType(marker, listContext);
        }

        private static string GetParagraphTypeFromStyle(
        Paragraph paragraph,
        IReadOnlyList<Paragraph> allParagraphs,
        int index,
        Styles styles)
        {
            var styleId = paragraph.ParagraphProperties?.ParagraphStyleId?.Val;
            var style = styles.Elements<Style>().FirstOrDefault(s => s.StyleId == styleId);
            var styleName = style?.StyleName?.Val?.ToString();

            return styleName switch
            {
                "heading 1" => ParagraphTypes.FirstH.ToString(),
                "heading 2" => ParagraphTypes.SecondH.ToString(),
                "heading 3" => ParagraphTypes.ThirdH.ToString(),
                _ when index > 0 && IsImage(allParagraphs[index - 1]) => ParagraphTypes.ImageCaption.ToString(),
                _ when index < allParagraphs.Count - 1 && IsTable(allParagraphs[index + 1]) => ParagraphTypes.TableCaption.ToString(),
                _ => ParagraphTypes.Normal.ToString()
            };
        }

        private static string GetListParagraphType(string marker, ListContext context)
        {
            _ = context.DetermineCurrentLevel(marker);
            return marker switch
            {
                "-" => ParagraphTypes.Dash.ToString(),
                "." => ParagraphTypes.Period.ToString(),
                ")" => ParagraphTypes.Bracket.ToString(),
                _ => ParagraphTypes.Normal.ToString()
            };
        }

        private static string GetListMarker(WordprocessingDocument doc, Paragraph p)
        {
            var numberingProperties = p.ParagraphProperties?.NumberingProperties;
            if (numberingProperties == null)
                return string.Empty;

            var numberingId = numberingProperties.NumberingId?.Val;
            var ilvl = numberingProperties.NumberingLevelReference?.Val?.Value;

            var numberingPart = doc.MainDocumentPart.NumberingDefinitionsPart;
            if (numberingPart == null)
                return string.Empty;

            var numInstance = numberingPart.Numbering.Elements<NumberingInstance>()
                .FirstOrDefault(n => n.NumberID?.Value == numberingId);

            if (numInstance == null)
                return string.Empty;

            var abstractNumId = numInstance.Elements<AbstractNumId>().FirstOrDefault()?.Val?.Value;

            var abstractNum = numberingPart.Numbering.Elements<AbstractNum>()
                .FirstOrDefault(a => a.AbstractNumberId?.Value == abstractNumId);

            if (abstractNum == null)
                return string.Empty;

            var level = abstractNum.Elements<Level>().FirstOrDefault(l => l.LevelIndex == ilvl);

            if (level == null)
                return string.Empty;

            var format = level.NumberingFormat?.Val;

            if (format == NumberFormatValues.Bullet)
                return "-";

            var lvlText = level.LevelText?.Val?.Value;

            if (string.IsNullOrEmpty(lvlText))
                return string.Empty;
            return lvlText.Contains(')') ? ")" : ".";
        }

        private static bool IsImage(Paragraph p) => p.Descendants<Drawing>().Any();
        private static bool IsTable(Paragraph p) => p.Ancestors<TableCell>().Any();

        /// <summary>
        /// Tracks current list nesting and helps determine list level.
        /// </summary>
        private class ListContext
        {
            private readonly Stack<string> _markerStack = new();

            public int DetermineCurrentLevel(string marker)
            {
                if (!_markerStack.Any())
                {
                    _markerStack.Push(marker);
                    return _markerStack.Count;
                }

                if (_markerStack.Peek() == marker)
                    return _markerStack.Count;

                if (_markerStack.Count >= 2 && _markerStack.Skip(1).First() == marker)
                {
                    _markerStack.Pop();
                    return _markerStack.Count;
                }

                _markerStack.Push(marker);

                if (_markerStack.Count > 3)
                {
                    _markerStack.Pop();
                }

                return _markerStack.Count;
            }

            public void Reset()
            {
                _markerStack.Clear();
            }
        }
    }
}