using FormatChanger.Models;

public interface IScoringService
{
    Task<EvaluationSystemModel?> GetScoringByIdAsync(long id);
    Task<List<EvaluationSystemModel>> GetScoringsAsync();
    Task<long> SaveScoringAsync(EvaluationSystemModel model);
    Task DeleteScoringAsync(long id);
}
