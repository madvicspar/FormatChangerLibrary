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
        public long HeadingLevel { get; set; }
        [ForeignKey("NextHeadingLevel")]
        /// <summary>
        /// Заголовок со следующим уровнем
        /// </summary>
        public long? NextHeadingLevelId { get; set; }
        public virtual TextSettingsModel TextSettings { get; set; }
        public virtual HeadingSettingsModel? NextHeadingLevel { get; set; }
    }
}
