using FormatChanger.Models;
using FormatChanger.Utilities.Data;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.EntityFrameworkCore;

public class TemplateService : ITemplateService
{
    private readonly ApplicationDbContext _context;

    public TemplateService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить шаблон по ID
    public async Task<FormattingTemplateModel> GetTemplateByIdAsync(long templateId)
    {
        return await _context.FormattingTemplates
            .Include(s => s.DocumentSettings)
            .Include(s => s.TextSettings)
            .Include(s => s.HeadingSettings).ThenInclude(t => t.TextSettings)
            .Include(s => s.ListSettings)
            .Include(s => s.ImageSettings)
            .Include(s => s.TableSettings).ThenInclude(c => c.CellSettings).ThenInclude(t => t.TextSettings)
            .Include(s => s.TableSettings).ThenInclude(h => h.HeaderSettings).ThenInclude(c => c.CellSettings).ThenInclude(t => t.TextSettings)
            .Include(s => s.TableSettings).ThenInclude(c => c.CaptionSettings).ThenInclude(t => t.TextSettings)
            .FirstOrDefaultAsync(t => t.Id == templateId);
    }
    // Получить список шаблонов
    // TODO: после реализации добавления шаблона поменять структуру бд и выводить только шаблоны, доступные текущему пользователю
    public async Task<List<FormattingTemplateModel>> GetTemplatesAsync()
    {
        return await _context.FormattingTemplates.ToListAsync();
    }

    // Применить шаблон к документу
    public void ApplyTemplateToDocument(WordprocessingDocument document, FormattingTemplateModel template)
    {
        // исправление?
    }
}