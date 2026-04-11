namespace FormatChanger.Models.FormattingModels
{
	public class HeadingLevelEditItem
	{
		public long Id { get; set; }
		public int HeadingLevel { get; set; }
		public bool StartOnNewPage { get; set; }
		public TextSettingsModel TextSettings { get; set; } = new();
	}
}
