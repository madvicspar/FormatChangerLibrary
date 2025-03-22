using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public class TextCorrectionStrategy : IElementCorrectionStrategy<TextSettingsModel>
    {
        public TextSettingsModel GetSettings(FormattingTemplateModel template)
        {
            return template.TextSettings;
        }
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
                    FirstLine = ((int)(settings.FirstLine * 567)).ToString()
                },
                new Justification { Val = JustificationConverter.Parse(settings.Justification) },
                new KeepNext { Val = settings.KeepWithNext }
            );
        }

        public void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            // TODO: сделать очистку формата (например, шрифты идут выше стилей)
            var settings = GetSettings(template);
            var stylePart = doc.MainDocumentPart?.StyleDefinitionsPart;
            if (stylePart?.Styles == null) return;

            var normalStyle = stylePart.Styles.Elements<Style>().FirstOrDefault(style => style.StyleName.Val == "Normal");
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

        public List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template)
        {
            throw new NotImplementedException();
        }
    }
}
