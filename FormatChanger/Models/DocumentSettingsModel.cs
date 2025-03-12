namespace FormatChanger.Models
{
    public class DocumentSettingsModel
    {
        public long Id { get; set; }
        /// <summary>
        /// Наличие нумерации страниц в документе
        /// </summary>
        public bool HasPageNumbers { get; set; }

        /// <summary>
        /// Наличие подписей к рисункам
        /// </summary>
        public bool HasImageCaptions { get; set; }

        /// <summary>
        /// Наличие подписей к таблицам
        /// </summary>
        public bool HasTableCaptions { get; set; }
    }
}
