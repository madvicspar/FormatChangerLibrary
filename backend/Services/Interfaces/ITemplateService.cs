using DocumentFormat.OpenXml.Packaging;

using FormatChanger.WebAPI.Models.FormattingModels;
public interface ITemplateService
{
	Task<FormattingTemplateModel> GetTemplateByIdAsync(long templateId);
	Task<List<FormattingTemplateModel>> GetTemplatesAsync();
	void ApplyTemplateToDocument(WordprocessingDocument document, FormattingTemplateModel template);
}
