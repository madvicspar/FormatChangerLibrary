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
        return await _context.FormattingTemplates.FindAsync(templateId);
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