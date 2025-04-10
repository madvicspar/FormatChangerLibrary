using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services.Strategies
{
    public class TextCorrectionStrategy : ElementCorrectionStrategyBase<TextSettingsModel>
    {
        public override TextSettingsModel GetSettings(FormattingTemplateModel template) =>
            template.TextSettings;

        public override void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            var settings = GetSettings(template);
            var styles = doc.MainDocumentPart?.StyleDefinitionsPart?.Styles;
            if (styles == null) return;

            var runProps = CreateRunProperties(settings);
            var paraProps = CreateParagraphProperties(settings);

            ApplyToStyle(styles, "Normal", runProps, paraProps);
        }

        public List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template)
        {
            throw new NotImplementedException();
        }
    }
}
