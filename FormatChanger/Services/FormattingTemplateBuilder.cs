using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;
using FormatChanger.Services.Interfaces;

namespace FormatChanger.Services
{
	public class FormattingTemplateBuilder : IFormattingTemplateBuilder
	{
		private readonly FormattingTemplateModel _template = new();

		public IFormattingTemplateBuilder SetTitle(string title)
		{
			_template.Title = title;
			return this;
		}

		public IFormattingTemplateBuilder SetTextSettings(TextSettingsModel textSettings)
		{
			_template.TextSettings = textSettings;
			_template.TextSettingsId = textSettings.Id;
			return this;
		}

		public IFormattingTemplateBuilder SetHeadingSettings(HeadingSettingsModel headingSettings)
		{
			_template.HeadingSettings = headingSettings;
			_template.HeadingSettingsId = headingSettings.Id;
			return this;
		}

		public IFormattingTemplateBuilder SetTableSettings(TableSettingsModel tableSettings)
		{
			_template.TableSettings = tableSettings;
			_template.TableSettingsId = tableSettings.Id;
			return this;
		}

		public IFormattingTemplateBuilder SetListSettings(ListSettingsModel listSettings)
		{
			_template.ListSettings = listSettings;
			_template.ListSettingsId = listSettings.Id;
			return this;
		}

		public IFormattingTemplateBuilder SetImageSettings(ImageSettingsModel imageSettings)
		{
			_template.ImageSettings = imageSettings;
			_template.ImageSettingsId = imageSettings.Id;
			return this;
		}

		public IFormattingTemplateBuilder SetDocumentSettings(DocumentSettingsModel documentSettings)
		{
			_template.DocumentSettings = documentSettings;
			_template.DocumentSettingsId = documentSettings.Id;
			return this;
		}

		public FormattingTemplateModel Build()
		{
			return _template;
		}
	}
}