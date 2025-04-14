using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services.Strategies
{
    public class ImageCorrectionStrategy : ElementCorrectionStrategyBase<ImageSettingsModel>
    {
        public override ImageSettingsModel GetSettings(FormattingTemplateModel template) =>
            template.ImageSettings;

        public override void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            var settings = GetSettings(template);
            var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().ToList();
            if (paragraphs == null) return;

            var paraProps = GetParagraphProperties(settings);

            foreach (var paragraph in paragraphs)
            {
                // TODO: скорее всего нужно так же исправлять настройки текста для параграфа с рисунками
                if (paragraph.Descendants<Drawing>().Any())
                    paragraph.ParagraphProperties = paraProps.CloneNode(true) as ParagraphProperties;
            }
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

        public List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template)
        {
            throw new NotImplementedException();
        }
    }
}
