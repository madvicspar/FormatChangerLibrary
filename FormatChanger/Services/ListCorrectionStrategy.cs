using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public class ListCorrectionStrategy : IElementCorrectionStrategy<ListSettingsModel>
    {
        public ListSettingsModel GetSettings(FormattingTemplateModel template)
        {
            return template.ListSettings;
        }

        public RunProperties GetRunProperties(ListSettingsModel settings)
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

        public ParagraphProperties GetParagraphProperties(ListSettingsModel settings)
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
            // тут меняем стиль - шрифт, отступы, интервалы и тд
            // проходимся по параграфам и меняем уровни нумерации на основе стиля соседних параграфов
            var settings = GetSettings(template);

            var paragraphList = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().ToList();

            
            foreach (var paragraph in paragraphList)
            {
                var paragraphProperties = paragraph.Elements<ParagraphProperties>().FirstOrDefault();

                if (paragraphProperties != null)
                {
                    var numberingProperties = paragraphProperties.Elements<NumberingProperties>().FirstOrDefault();
                    if (numberingProperties != null)
                    {
                        
                    }
                }
            }
        }
    }
}
