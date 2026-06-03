using DocumentFormat.OpenXml.Packaging;
using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;
using FormatChanger.Models.Helpers;

namespace FormatChanger.Services.Strategies
{
    public class ImageCaptionCorrectionStrategy : ElementCorrectionStrategyBase<ImageCaptionSettingsModel>
    {
        public override ImageCaptionSettingsModel GetSettings(FormattingTemplateModel template) =>
            template.ImageSettings.CaptionSettings as ImageCaptionSettingsModel;

        public override void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            // TODO: Add string pattern and maybe numbering
            // TODO: think about: need caption, but in classification there is no caption - what should we do?
            var settings = GetSettings(template);
            var styles = doc.MainDocumentPart?.StyleDefinitionsPart?.Styles;
            if (styles == null)
                return;

            var runProps = CreateRunProperties(settings.TextSettings);
            var paraProps = CreateParagraphProperties(settings.TextSettings);
            ApplyToStyle(styles, ParagraphTypes.ImageCaption.ToString(), runProps, paraProps);
        }

        public override List<string> CheckFormatting(ParagraphStyleProperties actual, FormattingTemplateModel template)
        {
			var expected = GetSettings(template);
			var issues = FormattingChecker.Check(actual, expected.TextSettings);
			issues.AddRange(FormattingChecker.CheckCaptionContent(actual, expected));
			return issues;
		}
	}
}
