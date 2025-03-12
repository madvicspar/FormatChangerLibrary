using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models
{
    public class CaptionSettingsModel
    {
        public long Id { get; set; }
        [ForeignKey("TextSettings")]
        public long TextSettingsId { get; set; }
        /// <summary>
        /// Шаблон текста для подписей, например "Рисунок 1 - "
        /// </summary>
        public string TextTemplate { get; set; }
        public virtual TextSettingsModel TextSettings { get; set; }
    }
}
