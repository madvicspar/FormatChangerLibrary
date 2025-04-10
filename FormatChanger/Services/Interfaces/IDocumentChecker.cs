using DocumentFormat.OpenXml.Packaging;
using FormatChanger.Models;

namespace FormatChanger.Services.Interfaces
{
    public interface IDocumentChecker
    {
        Task CheckAndCommentAsync(WordprocessingDocument doc, FormattingTemplateModel template, List<ParagraphModel> paragraphs, string[] types);
    }
}