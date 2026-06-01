using DocumentFormat.OpenXml.Packaging;
using FormatChanger.Models.FormattingModels;

public interface ITemplateService
{
    Task<FormattingTemplateModel> GetTemplateByIdAsync(long templateId);
    Task<List<FormattingTemplateModel>> GetTemplatesAsync();
    void ApplyTemplateToDocument(WordprocessingDocument document, FormattingTemplateModel template);
    Task<long> SaveTemplateAsync(FormattingTemplateModel model);
    Task DeleteTemplateAsync(long id);
}
