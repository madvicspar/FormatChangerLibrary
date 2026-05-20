using FormatChanger.Models.Helpers;

namespace FormatChanger.Models.FormattingModels
{
    public class NumberedListSettingsModel
    {
        public long Id { get; set; }
        public NumberedMarkerType Level1MarkerType { get; set; }
        public Ends Level1EndType { get; set; }
        public Ends Level2EndType { get; set; }
        public Ends Level3EndType { get; set; }
    }
}
