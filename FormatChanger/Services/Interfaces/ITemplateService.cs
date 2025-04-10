using FormatChanger.Models;
using DocumentFormat.OpenXml.Packaging;

public interface ITemplateService
{
    Task<FormattingTemplateModel> GetTemplateByIdAsync(long templateId);
    Task<List<FormattingTemplateModel>> GetTemplatesAsync();
    void ApplyTemplateToDocument(WordprocessingDocument document, FormattingTemplateModel template);
}
