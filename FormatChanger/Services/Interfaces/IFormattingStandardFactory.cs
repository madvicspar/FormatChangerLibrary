using FormatChanger.Models.FormattingModels;
using FormatChanger.Models.Helpers;

namespace FormatChanger.Services.Interfaces
{
	public interface IFormattingStandardFactory
	{
		TextSettingsModel CreateTextSettings();
		HeadingSettingsModel CreateHeadingSettings();
		TableSettingsModel CreateTableSettings();
		ImageSettingsModel CreateImageSettings();
		ListSettingsModel CreateListSettings();
	}
}