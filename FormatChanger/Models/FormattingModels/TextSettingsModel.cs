using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models.FormattingModels
{
    public class TextSettingsModel
    {
        public long Id { get; set; }
        public string Font { get; set; } = "Times New Roman";
        public string Color { get; set; } = "#000000";
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderscore { get; set; }
        public float FontSize { get; set; } = 14;

		/// <summary>
		/// Тип межстрочного интервала: single, onehalf, double, multiple
		/// </summary>
		[NotMapped]
		public string LineSpacingType { get; set; } = "single";

        /// <summary>
        /// Значение множителя интервала (используется только при LineSpacingType = "multiple")
        /// </summary>
        public float LineSpacing { get; set; } = 1;

        public float BeforeSpacing { get; set; }
        public float AfterSpacing { get; set; }
        public string Justification { get; set; } = "justify";
        public float Left { get; set; }
        public float Right { get; set; }

        /// <summary>
        /// Величина отступа/выступа первой строки в сантиметрах
        /// </summary>
        public float FirstLine { get; set; }

		/// <summary>
		/// Тип первой строки: none, indent, hanging
		/// </summary>
		[NotMapped]
		public string FirstLineType { get; set; } = "none";

        public bool KeepWithNext { get; set; }
    }
}
