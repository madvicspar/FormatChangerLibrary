using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models.FormattingModels
{
    public class ListSettingsModel
    {
        public long Id { get; set; }

        [ForeignKey("TextSettings")]
        public long TextSettingsId { get; set; }
        public virtual TextSettingsModel TextSettings { get; set; }

        [ForeignKey("BulletListSettings")]
        public long? BulletListSettingsId { get; set; }
        public virtual BulletListSettingsModel BulletListSettings { get; set; }

        [ForeignKey("NumberedListSettings")]
        public long? NumberedListSettingsId { get; set; }
        public virtual NumberedListSettingsModel NumberedListSettings { get; set; }

        public ListSettingsModel()
        {
            TextSettings = new TextSettingsModel();
            BulletListSettings = new BulletListSettingsModel();
            NumberedListSettings = new NumberedListSettingsModel();
        }
    }
}
