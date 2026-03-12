using System.ComponentModel.DataAnnotations.Schema;

using FormatChanger.WebAPI.Models.FormattingModels;

namespace FormatChanger.WebAPI.Models
{
	public class EvaluationResultsModel
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
