using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services.Interfaces
{
	public interface IFormattingTemplateBuilder
	{
		IFormattingTemplateBuilder SetTitle(string title);

		IFormattingTemplateBuilder SetTextSettings(TextSettingsModel textSettings);
		IFormattingTemplateBuilder SetHeadingSettings(HeadingSettingsModel headingSettings);
		IFormattingTemplateBuilder SetTableSettings(TableSettingsModel tableSettings);
		IFormattingTemplateBuilder SetListSettings(ListSettingsModel listSettings);
		IFormattingTemplateBuilder SetImageSettings(ImageSettingsModel imageSettings);
		IFormattingTemplateBuilder SetDocumentSettings(DocumentSettingsModel documentSettings);

		FormattingTemplateModel Build();
	}
}