using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models
{
    public class HeaderSettingsModel
    {
        public long Id { set; get; }
        [ForeignKey("CellSettings")]
        public long CellSettingsId { get; set; }
        public virtual CellSettingsModel CellSettings { get; set; }
        /// <summary>
        /// Наличие атрибута "Повторить строки заголовков"
        /// </summary>
        public bool HasRepetitions { get; set; }
    }
}
