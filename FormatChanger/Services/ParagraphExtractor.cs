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
            if (paragraphs == null) return null;

            var styles = doc.MainDocumentPart.StyleDefinitionsPart.Styles;

            return paragraphs
                .Select((p, i) => new ParagraphModel
                {
                    Paragraph = p,
                    Type = GetType(doc, p, paragraphs, i, styles)
                })
                .Where(p => !string.IsNullOrEmpty(p.Paragraph.InnerText) && !p.Paragraph.Ancestors<TableCell>().Any()).ToList();
        }

        private string GetType(WordprocessingDocument doc, Paragraph p, List<Paragraph> all, int i, Styles styles)
        {
            var styleId = p.ParagraphProperties?.ParagraphStyleId?.Val;
            var style = styles.Elements<Style>().FirstOrDefault(x => x.StyleId == styleId);
            var name = style?.StyleName?.Val?.ToString();

            var markerType = GetListMarker(doc, p);
            // УРОВНИ НИЖЕ ЧЕТВЕРТОГО ИДУТ КАК 3 УРОВЕНЬ

            return name switch
            {
                "heading 1" => ParagraphTypes.FirstH.ToString(),
                "heading 2" => ParagraphTypes.SecondH.ToString(),
                "heading 3" => ParagraphTypes.ThirdH.ToString(),
                _ when i > 0 && IsImage(all[i - 1]) => ParagraphTypes.ImageCaption.ToString(),
                _ when i < all.Count - 1 && IsTable(all[i + 1]) => ParagraphTypes.TableCaption.ToString(),
                _ when markerType == "-" => ParagraphTypes.Dash.ToString(),
                _ when markerType == "." => ParagraphTypes.Period.ToString(),
                _ when markerType == ")" => ParagraphTypes.Bracket.ToString(),
                _ => ParagraphTypes.Normal.ToString()
            };
        }

        private string GetListMarker(WordprocessingDocument doc, Paragraph p)
        {
            var numberingProperties = p.ParagraphProperties?.NumberingProperties;
            try
            {
                var numberingId = numberingProperties.NumberingId?.Val;
                var ilvl = numberingProperties.NumberingLevelReference?.Val?.Value;

                var numberingPart = doc.MainDocumentPart.NumberingDefinitionsPart ?? doc.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>();

                var numInstance = numberingPart.Numbering.Elements<NumberingInstance>()
                    .FirstOrDefault(n => n.NumberID.Value == numberingId);

                var abstractNumId = numInstance.Elements<AbstractNumId>().FirstOrDefault()?.Val?.Value;

                var abstractNum = numberingPart.Numbering.Elements<AbstractNum>()
                    .FirstOrDefault(a => a.AbstractNumberId.Value == abstractNumId);

                var level = abstractNum.Elements<Level>().FirstOrDefault(l => l.LevelIndex == ilvl);

                var format = level.NumberingFormat?.Val;

                if (format == NumberFormatValues.Bullet)
                    return "-";
                var lvlText = level.LevelText?.Val?.Value;
                if (string.IsNullOrEmpty(lvlText))
                    return string.Empty;
                return lvlText.Contains(')') ? ")" : ".";
            }
            catch
            {
                return string.Empty;
            }
        }

        private bool IsImage(Paragraph p) => p.Descendants<Drawing>().Any();
        private bool IsTable(Paragraph p) => p.Ancestors<TableCell>().Any();
    }
}