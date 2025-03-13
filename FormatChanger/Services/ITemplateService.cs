using FormatChanger.Models;
using DocumentFormat.OpenXml.Packaging;

public interface ITemplateService
{
    Task<FormattingTemplateModel> GetTemplateByIdAsync(long templateId);
    void ApplyTemplateToDocument(WordprocessingDocument document, FormattingTemplateModel template);
}
