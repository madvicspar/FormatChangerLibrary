using DocumentFormat.OpenXml.Packaging;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public interface IDocumentCorrector
    {
        Task ApplyAllStrategiesAsync(WordprocessingDocument doc, FormattingTemplateModel template, List<ParagraphModel> paragraphs);
    }
}