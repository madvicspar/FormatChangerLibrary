using FormatChanger.Models;
using FormatChanger.Utilities.Data;
using DocumentFormat.OpenXml.Packaging;

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

    // Применить шаблон к документу
    public void ApplyTemplateToDocument(WordprocessingDocument document, FormattingTemplateModel template)
    {
        // исправление?
    }
}