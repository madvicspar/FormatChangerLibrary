using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models
{
    public class HeadingSettingsModel
    {
        public long Id { get; set; }
        [ForeignKey("TextSettings")]
        public long TextSettingsId { get; set; }
        /// <summary>
        /// Атрибут "С новой страницы"
        /// </summary>
        public bool StartOnNewPage { get; set; }
        /// <summary>
        /// Уровень заголовка
        /// </summary>
        public int HeadingLevel { get; set; }
        // ссылка на следующий/предудыщий уровень?
        public virtual TextSettingsModel TextSettings { get; set; }
    }
}
