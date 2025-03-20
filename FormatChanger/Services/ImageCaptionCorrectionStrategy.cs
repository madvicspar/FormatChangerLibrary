using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public class ImageCaptionCorrectionStrategy : IElementCorrectionStrategy<ImageCaptionSettingsModel>
    {
        public ImageCaptionSettingsModel GetSettings(FormattingTemplateModel template)
        {
            return template.ImageSettings.CaptionSettings as ImageCaptionSettingsModel;
        }
        public RunProperties GetRunProperties(ImageCaptionSettingsModel settings)
        {
            return new RunProperties(
                new RunFonts { Ascii = settings.TextSettings.Font, HighAnsi = settings.TextSettings.Font },
                new Color { Val = settings.TextSettings.Color },
                new Bold { Val = settings.TextSettings.IsBold },
                new Italic { Val = settings.TextSettings.IsItalic },
                new Underline { Val = settings.TextSettings.IsUnderscore ? UnderlineValues.Single : UnderlineValues.None },
                new FontSize { Val = (settings.TextSettings.FontSize * 2).ToString() }
            );
        }

        public ParagraphProperties GetParagraphProperties(ImageCaptionSettingsModel settings)
        {
            var paragraphProperties = new ParagraphProperties(
            new SpacingBetweenLines
            {
                Line = settings.TextSettings.LineSpacing.ToString(),
                LineRule = LineSpacingRuleValues.Auto,
                Before = settings.TextSettings.BeforeSpacing.ToString(),
                After = settings.TextSettings.AfterSpacing.ToString()
            },
            new Indentation
            {
                Left = settings.TextSettings.Left.ToString(),
                Right = settings.TextSettings.Right.ToString(),
                FirstLine = settings.TextSettings.FirstLine.ToString()
            },
            new Justification { Val = JustificationConverter.Parse(settings.TextSettings.Justification) },
            new KeepNext { Val = settings.TextSettings.KeepWithNext }
            );
            return paragraphProperties;
        }

        public void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            // TODO: Add string pattern and maybe numbering
            // TODO: think about: need caption, but in classification there is no caption - what should we do?
            // TODO: Change logic of getting image signature paragraphs
            var settings = GetSettings(template);
            var stylePart = doc.MainDocumentPart?.StyleDefinitionsPart;
            if (stylePart?.Styles == null) return;

            var style = stylePart.Styles.Elements<Style>().FirstOrDefault(style => style.StyleId == ParagraphTypes.ImageCaption.ToString());
            if (style == null)
            {
                Console.WriteLine($"Style {ParagraphTypes.ImageCaption} not found.");
                return;
            }

            style.RemoveAllChildren<StyleRunProperties>();
            style.RemoveAllChildren<StyleParagraphProperties>();

            style.AppendChild(new StyleRunProperties(GetRunProperties(settings)));
            style.AppendChild(new StyleParagraphProperties(GetParagraphProperties(settings)));
        }
    }
}
