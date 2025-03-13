using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public class ImageCorrectionStrategy /*: IElementCorrectionStrategy<ImageSettingsModel>*/
    {
        public RunProperties GetRunProperties(ImageSettingsModel settings)
        {
            return new RunProperties();
        }

        public ParagraphProperties GetParagraphProperties(ImageSettingsModel settings)
        {
            return new ParagraphProperties(
            new Justification { Val = JustificationConverter.Parse(settings.Justification) },
            new SpacingBetweenLines
            {
                Line = settings.LineSpacing.ToString(),
                LineRule = LineSpacingRuleValues.Auto,
                Before = settings.BeforeSpacing.ToString(),
                After = settings.AfterSpacing.ToString()
            },
            new Indentation
            {
                Left = settings.Left.ToString(),
                Right = settings.Right.ToString(),
                FirstLine = settings.FirstLine.ToString()
            },
            new KeepNext { Val = settings.KeepWithNext });
        }

        public void ApplyCorrection(WordprocessingDocument doc, ImageSettingsModel settings)
        {
            var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().ToList();
            if (paragraphs == null) return;

            foreach (var paragraph in paragraphs)
            {
                var drawings = paragraph.Descendants<Drawing>().ToList();
                if (drawings.Any())
                {
                    paragraph.ParagraphProperties = GetParagraphProperties(settings);
                }
            }
        }
    }
}
