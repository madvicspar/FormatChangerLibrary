using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public class TextCorrectionStrategy /*: IElementCorrectionStrategy<TextSettingsModel>*/
    {
        public RunProperties GetRunProperties(TextSettingsModel settings)
        {
            return new RunProperties(
                new RunFonts { Ascii = settings.Font, HighAnsi = settings.Font },
                new Color { Val = settings.Color },
                new Bold { Val = settings.IsBold },
                new Italic { Val = settings.IsItalic },
                new Underline { Val = settings.IsUnderscore ? UnderlineValues.Single : UnderlineValues.None },
                new FontSize { Val = (settings.FontSize * 2).ToString() }
            );
        }

        public ParagraphProperties GetParagraphProperties(TextSettingsModel settings)
        {
            return new ParagraphProperties(
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
                new Justification { Val = JustificationConverter.Parse(settings.Justification) },
                new KeepNext { Val = settings.KeepWithNext }
            );
        }

        public void ApplyCorrection(WordprocessingDocument doc, TextSettingsModel settings)
        {
            var stylePart = doc.MainDocumentPart?.StyleDefinitionsPart;
            if (stylePart?.Styles == null) return;

            var normalStyle = stylePart.Styles.Elements<Style>().FirstOrDefault(style => style.StyleId == "Normal");
            if (normalStyle == null)
            {
                Console.WriteLine("Style 'Normal' not found.");
                return;
            }

            normalStyle.RemoveAllChildren<StyleRunProperties>();
            normalStyle.RemoveAllChildren<StyleParagraphProperties>();

            normalStyle.AppendChild(new StyleRunProperties(GetRunProperties(settings)));
            normalStyle.AppendChild(new StyleParagraphProperties(GetParagraphProperties(settings)));
        }
    }
}
