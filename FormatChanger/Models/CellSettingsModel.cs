using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models
{
    public class CellSettingsModel
    {
        public long Id { set; get; }
        [ForeignKey("TextSettings")]
        public long TextSettingsId { get; set; }
        public virtual TextSettingsModel TextSettings { get; set; }

        /// <summary>
        /// Выравнивание по высоте ячеек
        /// </summary>
        public string VerticalAlignment { get; set; }

        /// <summary>
        /// Левое поле ячеек
        /// </summary>
        public int LeftPadding { get; set; }

        /// <summary>
        /// Правое поле ячеек
        /// </summary>
        public int RightPadding { get; set; }

        /// <summary>
        /// Нижнее поле ячеек
        /// </summary>
        public int BottomPadding { get; set; }

        /// <summary>
        /// Верхнее поле ячеек
        /// </summary>
        public int TopPadding { get; set; }
    }
}
