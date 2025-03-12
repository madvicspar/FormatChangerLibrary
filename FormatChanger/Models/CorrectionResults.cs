using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models
{
    public class CorrectionResults
    {
        public long Id { get; set; }

        [ForeignKey("FormattingTemplate")]
        public long FormattingTemplateId { get; set; }
        /// <summary>
        /// Оригинальный документ
        /// </summary>

        [ForeignKey("Document")]
        public long DocumentId { get; set; }
        /// <summary>
        /// Исправленный документ
        /// </summary>

        [ForeignKey("CorrectedDocument")]
        public int CorrectedDocumentId { get; set; }

        public virtual FormattingTemplateModel FormattingTemplate { get; set; }
        public virtual DocumentModel Document { get; set; }
        public virtual DocumentModel CorrectedDocument { get; set; }
    }
}
