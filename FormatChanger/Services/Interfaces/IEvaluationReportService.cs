using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services.Interfaces
{
    public interface IEvaluationReportService
    {
        byte[] GenerateReport(EvaluationSystemModel scoring, FormattingTemplateModel template);
    }
}
