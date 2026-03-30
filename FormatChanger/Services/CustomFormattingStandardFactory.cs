using FormatChanger.Models.FormattingModels;
using FormatChanger.Models.Helpers;
using FormatChanger.Services.Interfaces;

namespace FormatChanger.Services
{
	public class CustomFormattingStandardFactory : IFormattingStandardFactory
	{
		public TextSettingsModel CreateTextSettings() => new()
		{
			Font = "Times New Roman",
			Color = "000000",
			IsBold = false,
			IsItalic = false,
			IsUnderscore = false,
			FontSize = 13,
			LineSpacing = 360,
			BeforeSpacing = 0,
			AfterSpacing = 0,
			Justification = "Both",
			Left = 0,
			Right = 0,
			FirstLine = 1.25f,
			KeepWithNext = false
		};

		public HeadingSettingsModel CreateHeadingSettings()
		{
			var h3 = new HeadingSettingsModel
			{
				HeadingLevel = 3,
				StartOnNewPage = false,
				TextSettings = new TextSettingsModel
				{
					Font = "Times New Roman",
					Color = "000000",
					IsBold = true,
					FontSize = 13,
					LineSpacing = 240,
					BeforeSpacing = 160,
					AfterSpacing = 80,
					Justification = "Center",
					FirstLine = 0,
					KeepWithNext = true
				}
			};

			var h2 = new HeadingSettingsModel
			{
				HeadingLevel = 2,
				StartOnNewPage = false,
				NextHeadingLevel = h3,
				TextSettings = new TextSettingsModel
				{
					Font = "Times New Roman",
					Color = "000000",
					IsBold = true,
					FontSize = 14,
					LineSpacing = 240,
					BeforeSpacing = 240,
					AfterSpacing = 120,
					Justification = "Center",
					FirstLine = 0,
					KeepWithNext = true
				}
			};

			return new HeadingSettingsModel
			{
				HeadingLevel = 1,
				StartOnNewPage = true,
				NextHeadingLevel = h2,
				TextSettings = new TextSettingsModel
				{
					Font = "Times New Roman",
					Color = "000000",
					IsBold = true,
					FontSize = 16,
					LineSpacing = 240,
					BeforeSpacing = 0,
					AfterSpacing = 240,
					Justification = "Center",
					FirstLine = 0,
					KeepWithNext = true
				}
			};
		}

		public TableSettingsModel CreateTableSettings()
		{
			var tableCaption = new TableCaptionSettingsModel
			{
				TextTemplate = "Таблица\\s+\\d+\\s+-\\s+(.+)",
				TextSettings = new TextSettingsModel
				{
					Font = "Times New Roman",
					FontSize = 13,
					LineSpacing = 240,
					BeforeSpacing = 120,
					Justification = "Both",
					KeepWithNext = true
				}
			};

			var cellSettings = new CellSettingsModel
			{
				VerticalAlignment = "Top",
				TopPadding = 2,
				LeftPadding = 2,
				BottomPadding = 2,
				RightPadding = 2,
				TextSettings = new TextSettingsModel
				{
					Font = "Times New Roman",
					FontSize = 11,
					Justification = "Left"
				}
			};

			var headerCellSettings = new CellSettingsModel
			{
				VerticalAlignment = "Top",
				TopPadding = 2,
				LeftPadding = 2,
				BottomPadding = 2,
				RightPadding = 2,
				TextSettings = new TextSettingsModel
				{
					Font = "Times New Roman",
					FontSize = 11,
					Justification = "Center",
					KeepWithNext = true
				}
			};

			var headerSettings = new HeaderSettingsModel
			{
				CellSettings = headerCellSettings,
				HasRepetitions = true
			};

			return new TableSettingsModel
			{
				BeforeSpacing = 0,
				AfterSpacing = 120,
				CaptionSettings = tableCaption,
				CellSettings = cellSettings,
				HeaderSettings = headerSettings
			};
		}

		public ImageSettingsModel CreateImageSettings()
		{
			var caption = new ImageCaptionSettingsModel
			{
				TextTemplate = "Рисунок\\s+\\d+\\s+-\\s+(.+)",
				TextSettings = new TextSettingsModel
				{
					Font = "Times New Roman",
					FontSize = 11,
					IsBold = true,
					IsItalic = true,
					Justification = "Center"
				}
			};

			return new ImageSettingsModel
			{
				CaptionSettings = caption,
				LineSpacing = 240,
				BeforeSpacing = 120,
				AfterSpacing = 0,
				Justification = "Center",
				KeepWithNext = true
			};
		}

		public ListSettingsModel CreateListSettings() => new()
		{
			EndType = Ends.Semicolon,
			IsNumeric = false,
			MarkerType = "-",
			ListLevel = 0,
			TextSettings = new TextSettingsModel
			{
				Font = "Times New Roman",
				FontSize = 13,
				LineSpacing = 360,
				Justification = "Both",
				Left = 1.5f,
				Right = 0.5f,
				FirstLine = 1.25f
			}
		};
	}
}