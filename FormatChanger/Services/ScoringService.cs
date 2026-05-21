using FormatChanger.Models;
using FormatChanger.Utilities.Data;
using Microsoft.EntityFrameworkCore;

public class ScoringService : IScoringService
{
    private readonly ApplicationDbContext _context;

    public ScoringService(ApplicationDbContext context) => _context = context;

    public async Task<EvaluationSystemModel?> GetScoringByIdAsync(long id)
        => await _context.EvaluationSystems.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<EvaluationSystemModel>> GetScoringsAsync()
        => await _context.EvaluationSystems.ToListAsync();

    public async Task<long> SaveScoringAsync(EvaluationSystemModel model)
    {
        if (model.Id == 0)
        {
            _context.EvaluationSystems.Add(model);
        }
        else
        {
            var existing = await _context.EvaluationSystems.FindAsync(model.Id)
                ?? throw new KeyNotFoundException($"Scoring system {model.Id} not found");
            existing.Title = model.Title;
            existing.TextWeight = model.TextWeight;
            existing.HeaderWeight = model.HeaderWeight;
            existing.ListWeight = model.ListWeight;
            existing.ImageWeight = model.ImageWeight;
            existing.TableWeight = model.TableWeight;
            existing.FreeCoefficient = model.FreeCoefficient;
            existing.RuleWeightsJson = model.RuleWeightsJson;
        }
        await _context.SaveChangesAsync();
        return model.Id;
    }

    public async Task DeleteScoringAsync(long id)
    {
        var s = await _context.EvaluationSystems.FindAsync(id);
        if (s == null) return;
        _context.EvaluationSystems.Remove(s);
        await _context.SaveChangesAsync();
    }
}
