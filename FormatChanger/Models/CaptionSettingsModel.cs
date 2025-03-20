using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models
{
    public class ICaptionSettingsModel
    {
        public long Id { get; set; }
        [ForeignKey("TextSettings")]
        public long TextSettingsId { get; set; }
        /// <summary>
        /// Шаблон текста для подписей, например "Рисунок 1 - "
        /// </summary>
        public string TextTemplate { get; set; }
        public string Discriminator { get; set; }
        public virtual TextSettingsModel TextSettings { get; set; }
    }
    public class ImageCaptionSettingsModel : ICaptionSettingsModel { }
    public class TableCaptionSettingsModel : ICaptionSettingsModel { }
}
