using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;
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
                    Type = GetType(p, paragraphs, i, styles)
                })
                .Where(p => !string.IsNullOrEmpty(p.Paragraph.InnerText) && !p.Paragraph.Ancestors<TableCell>().Any()).ToList();
        }

        private string GetType(Paragraph p, List<Paragraph> all, int i, Styles styles)
        {
            var styleId = p.ParagraphProperties?.ParagraphStyleId?.Val;
            var style = styles.Elements<Style>().FirstOrDefault(x => x.StyleId == styleId);
            var name = style?.StyleName?.Val?.ToString();

            return name switch
            {
                "heading 1" => ParagraphTypes.FirstH.ToString(),
                "heading 2" => ParagraphTypes.SecondH.ToString(),
                "heading 3" => ParagraphTypes.ThirdH.ToString(),
                _ when i > 0 && IsImage(all[i - 1]) => ParagraphTypes.ImageCaption.ToString(),
                _ when i < all.Count - 1 && IsTable(all[i + 1]) => ParagraphTypes.TableCaption.ToString(),
                _ => ParagraphTypes.Normal.ToString()
            };
        }

        private bool IsImage(Paragraph p) => p.Descendants<Drawing>().Any();
        private bool IsTable(Paragraph p) => p.Ancestors<TableCell>().Any();
    }
}