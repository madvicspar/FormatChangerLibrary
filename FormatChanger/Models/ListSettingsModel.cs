using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models
{
    public class ListSettingsModel
    {
        public long Id { get; set; }
        [ForeignKey("TextSettings")]
        public long TextSettingsId { get; set; }
        /// <summary>
        /// Является ли список нумерованным или маркированным
        /// <para>True - если список нумерованный, False - если маркированный.</para>
        /// </summary>
        public bool IsNumeric { get; set; }
        public string? MarkerType { get; set; }
        public int ListLevel { get; set; }
        /// <summary>
        /// Знак препинания на конце элемента списка
        /// </summary>
        public virtual TextSettingsModel TextSettings { get; set; }
        public Ends EndType { get; set; }
    }
}
