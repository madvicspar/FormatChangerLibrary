using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models.FormattingModels;
using FormatChanger.Models.Helpers;

namespace FormatChanger.Services.Strategies
{
    public class TableCaptionCorrectionStrategy : ElementCorrectionStrategyBase<TableCaptionSettingsModel>
    {
        public override TableCaptionSettingsModel GetSettings(FormattingTemplateModel template) =>
            template.TableSettings.CaptionSettings as TableCaptionSettingsModel;

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
            ApplyToStyle(styles, ParagraphTypes.TableCaption.ToString(), runProps, paraProps);
        }

        public List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template)
        {
            throw new NotImplementedException();
        }
    }
}
