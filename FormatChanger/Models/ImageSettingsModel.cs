namespace FormatChanger.Models
{
    public class ImageSettingsModel
    {
        public long Id { get; set; }
        /// <summary>
        /// Междустрочный интервал в какой-то единице измерения
        /// </summary>
        public float LineSpacing { get; set; }
        /// <summary>
        /// Интервал до
        /// </summary>
        public float BeforeSpacing { get; set; }
        /// <summary>
        /// Интервал после
        /// </summary>
        public float AfterSpacing { get; set; }
        /// <summary>
        /// Выравнивание
        /// </summary>
        public string Justification { get; set; }
        /// <summary>
        /// Отступ слева
        /// </summary>
        public float Left { get; set; }
        /// <summary>
        /// Отступ справа
        /// </summary>
        public float Right { get; set; }
        /// <summary>
        /// Отступ первой строки
        /// </summary>
        public float FirstLine { get; set; }
        /// <summary>
        /// Атрибут "Не отрывать от следующего"
        /// </summary>
        public bool KeepWithNext { get; set; }
    }
}
