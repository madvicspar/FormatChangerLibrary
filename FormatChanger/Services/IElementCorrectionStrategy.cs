using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    // TODO: add other style strategies and glodal document strategy
    public interface IElementCorrectionStrategy<T>
    {
        T GetSettings(FormattingTemplateModel template);
        void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template);
        RunProperties GetRunProperties(T settings);
        ParagraphProperties GetParagraphProperties(T settings);
        public List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template);
    }
}