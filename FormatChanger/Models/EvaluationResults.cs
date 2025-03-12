using System.ComponentModel.DataAnnotations.Schema;

namespace FormatChanger.Models
{
    public class EvaluationResults
    {
        public long Id { get; set; }
        public int Score { get; set; }
        [ForeignKey("FormattingTemplate")]
        public long FormattingTemplateId { get; set; }
        [ForeignKey("Document")]
        public long DocumentId { get; set; }
        [ForeignKey("EvaluationSystem")]
        public long EvaluationSystemModelId { get; set; }

        public virtual FormattingTemplateModel FormattingTemplate { get; set; }
        public virtual DocumentModel Document { get; set; }
        public virtual EvaluationSystemModel EvaluationSystem { get; set; }
    }
}
