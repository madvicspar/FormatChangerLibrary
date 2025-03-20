using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models
{
    public class TableSettingsModel
    {
        public long Id { get; set; }
        /// <summary>
        /// Интервал до
        /// </summary>
        public float BeforeSpacing { get; set; }
        /// <summary>
        /// Интервал после
        /// </summary>
        public float AfterSpacing { get; set; }
        [ForeignKey("CaptionSettings")]
        public long CaptionSettingsId { get; set; }
        [ForeignKey("CellSettings")]
        public long CellSettingsId { get; set; }
        [ForeignKey("HeaderSettings")]
        public long HeaderSettingsModelId { get; set; }
        public virtual ICaptionSettingsModel CaptionSettings { get; set; }
        public virtual CellSettingsModel CellSettings { get; set; }
        public virtual HeaderSettingsModel HeaderSettings { get; set; }
    }
}
