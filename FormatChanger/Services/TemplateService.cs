using DocumentFormat.OpenXml.Packaging;
using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;
using FormatChanger.Models.Helpers;
using FormatChanger.Utilities.Data;
using Microsoft.EntityFrameworkCore;

public class TemplateService : ITemplateService
{
    private readonly ApplicationDbContext _context;

    public TemplateService(ApplicationDbContext context) => _context = context;

    // ─── Read ────────────────────────────────────────────────────────────────

    public async Task<FormattingTemplateModel> GetTemplateByIdAsync(long templateId)
    {
        return await _context.FormattingTemplates
            .Include(t => t.DocumentSettings)
            .Include(t => t.TextSettings)
            .Include(t => t.HeadingSettings).ThenInclude(h => h.TextSettings)
            .Include(t => t.HeadingSettings).ThenInclude(h => h.NextHeadingLevel).ThenInclude(h => h.TextSettings)
            .Include(t => t.HeadingSettings).ThenInclude(h => h.NextHeadingLevel)
                .ThenInclude(h => h.NextHeadingLevel).ThenInclude(h => h.TextSettings)
            .Include(t => t.ListSettings).ThenInclude(l => l.TextSettings)
            .Include(t => t.ListSettings).ThenInclude(l => l.BulletListSettings)
            .Include(t => t.ListSettings).ThenInclude(l => l.NumberedListSettings)
            .Include(t => t.ImageSettings).ThenInclude(img => img.CaptionSettings).ThenInclude(c => c.TextSettings)
            .Include(t => t.TableSettings).ThenInclude(ts => ts.CaptionSettings).ThenInclude(c => c.TextSettings)
            .Include(t => t.TableSettings).ThenInclude(ts => ts.CellSettings).ThenInclude(c => c.TextSettings)
            .Include(t => t.TableSettings).ThenInclude(ts => ts.HeaderSettings)
                .ThenInclude(h => h.CellSettings).ThenInclude(c => c.TextSettings)
            .FirstOrDefaultAsync(t => t.Id == templateId);
    }

    public async Task<List<FormattingTemplateModel>> GetTemplatesAsync()
        => await _context.FormattingTemplates.ToListAsync();

    public void ApplyTemplateToDocument(WordprocessingDocument document, FormattingTemplateModel template) { }

    // ─── Write ───────────────────────────────────────────────────────────────

    public async Task<long> SaveTemplateAsync(FormattingTemplateModel model)
        => model.Id == 0 ? await CreateAsync(model) : await UpdateAsync(model);

    public async Task DeleteTemplateAsync(long id)
    {
        var t = await _context.FormattingTemplates
            .Include(x => x.TextSettings)
            .Include(x => x.DocumentSettings)
            .Include(x => x.HeadingSettings).ThenInclude(h => h.TextSettings)
            .Include(x => x.HeadingSettings).ThenInclude(h => h.NextHeadingLevel).ThenInclude(h => h.TextSettings)
            .Include(x => x.HeadingSettings).ThenInclude(h => h.NextHeadingLevel)
                .ThenInclude(h => h.NextHeadingLevel).ThenInclude(h => h.TextSettings)
            .Include(x => x.ListSettings).ThenInclude(l => l.TextSettings)
            .Include(x => x.ListSettings).ThenInclude(l => l.BulletListSettings)
            .Include(x => x.ListSettings).ThenInclude(l => l.NumberedListSettings)
            .Include(x => x.TableSettings).ThenInclude(ts => ts.CaptionSettings).ThenInclude(c => c.TextSettings)
            .Include(x => x.TableSettings).ThenInclude(ts => ts.CellSettings).ThenInclude(c => c.TextSettings)
            .Include(x => x.TableSettings).ThenInclude(ts => ts.HeaderSettings)
                .ThenInclude(h => h.CellSettings).ThenInclude(c => c.TextSettings)
            .Include(x => x.ImageSettings).ThenInclude(img => img.CaptionSettings).ThenInclude(c => c.TextSettings)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (t == null) return;

        // Break heading chain links so each node can be deleted independently
        var headings = FlattenChain(t.HeadingSettings);
        foreach (var h in headings) { h.NextHeadingLevelId = null; h.NextHeadingLevel = null; }
        await _context.SaveChangesAsync();

        // Remove template row first (it is the dependent, so no cascade issues)
        _context.FormattingTemplates.Remove(t);
        await _context.SaveChangesAsync();

        // Remove orphaned settings trees
        RemoveIfNotNull(_context.TextSettings, t.TextSettings);
        RemoveIfNotNull(_context.DocumentSettings, t.DocumentSettings);

        foreach (var h in headings)
        {
            RemoveIfNotNull(_context.TextSettings, h.TextSettings);
            _context.HeadingSettings.Remove(h);
        }

        var ls = t.ListSettings;
        if (ls != null)
        {
            RemoveIfNotNull(_context.TextSettings, ls.TextSettings);
            if (ls.BulletListSettings != null) _context.Set<BulletListSettingsModel>().Remove(ls.BulletListSettings);
            if (ls.NumberedListSettings != null) _context.Set<NumberedListSettingsModel>().Remove(ls.NumberedListSettings);
            _context.ListSettings.Remove(ls);
        }

        var ts = t.TableSettings;
        if (ts != null)
        {
            RemoveIfNotNull(_context.TextSettings, ts.CaptionSettings?.TextSettings);
            if (ts.CaptionSettings != null) _context.CaptionSettings.Remove(ts.CaptionSettings);
            RemoveIfNotNull(_context.TextSettings, ts.CellSettings?.TextSettings);
            if (ts.CellSettings != null) _context.CellSettings.Remove(ts.CellSettings);
            RemoveIfNotNull(_context.TextSettings, ts.HeaderSettings?.CellSettings?.TextSettings);
            if (ts.HeaderSettings?.CellSettings != null) _context.CellSettings.Remove(ts.HeaderSettings.CellSettings);
            if (ts.HeaderSettings != null) _context.HeaderSettings.Remove(ts.HeaderSettings);
            _context.TableSettings.Remove(ts);
        }

        var imgs = t.ImageSettings;
        if (imgs != null)
        {
            RemoveIfNotNull(_context.TextSettings, imgs.CaptionSettings?.TextSettings);
            if (imgs.CaptionSettings != null) _context.CaptionSettings.Remove(imgs.CaptionSettings);
            _context.ImageSettings.Remove(imgs);
        }

        await _context.SaveChangesAsync();
    }

    // ─── Create ──────────────────────────────────────────────────────────────

    private async Task<long> CreateAsync(FormattingTemplateModel model)
    {
        model.HeadingSettings = BuildChain(model.HeadingLevelsEdit);

        // CaptionSettings must be the correct derived TPH types
        if (model.TableSettings != null)
            model.TableSettings.CaptionSettings = ToTableCaption(model.TableSettings.CaptionSettings);
        if (model.ImageSettings != null)
            model.ImageSettings.CaptionSettings = ToImageCaption(model.ImageSettings.CaptionSettings);

        _context.FormattingTemplates.Add(model);
        await _context.SaveChangesAsync();
        return model.Id;
    }

    // ─── Update ──────────────────────────────────────────────────────────────

    private async Task<long> UpdateAsync(FormattingTemplateModel model)
    {
        var e = await GetTemplateByIdAsync(model.Id)
                ?? throw new KeyNotFoundException($"Template {model.Id} not found");

        e.Title = model.Title;
        CopyDoc(e.DocumentSettings, model.DocumentSettings);
        CopyText(e.TextSettings, model.TextSettings);
        await UpdateChainAsync(e, model.HeadingLevelsEdit);

        var ls = model.ListSettings;
        if (ls != null)
        {
            CopyText(e.ListSettings.TextSettings, ls.TextSettings);
            if (ls.BulletListSettings != null)
                e.ListSettings.BulletListSettings.EndType = ls.BulletListSettings.EndType;
            if (ls.NumberedListSettings != null)
            {
                e.ListSettings.NumberedListSettings.Level1MarkerType = ls.NumberedListSettings.Level1MarkerType;
                e.ListSettings.NumberedListSettings.Level1EndType = ls.NumberedListSettings.Level1EndType;
                e.ListSettings.NumberedListSettings.Level2EndType = ls.NumberedListSettings.Level2EndType;
                e.ListSettings.NumberedListSettings.Level3EndType = ls.NumberedListSettings.Level3EndType;
            }
        }

        var ts = model.TableSettings;
        if (ts != null)
        {
            e.TableSettings.BeforeSpacing = ts.BeforeSpacing;
            e.TableSettings.AfterSpacing = ts.AfterSpacing;
            if (ts.CaptionSettings != null)
            {
                e.TableSettings.CaptionSettings.TextTemplate = ts.CaptionSettings.TextTemplate;
                e.TableSettings.CaptionSettings.Separator = ts.CaptionSettings.Separator;
                e.TableSettings.CaptionSettings.Position = ts.CaptionSettings.Position;
                CopyText(e.TableSettings.CaptionSettings.TextSettings, ts.CaptionSettings.TextSettings);
            }
            if (ts.CellSettings != null)
            {
                e.TableSettings.CellSettings.VerticalAlignment = ts.CellSettings.VerticalAlignment;
                e.TableSettings.CellSettings.TopPadding = ts.CellSettings.TopPadding;
                e.TableSettings.CellSettings.BottomPadding = ts.CellSettings.BottomPadding;
                e.TableSettings.CellSettings.LeftPadding = ts.CellSettings.LeftPadding;
                e.TableSettings.CellSettings.RightPadding = ts.CellSettings.RightPadding;
                CopyText(e.TableSettings.CellSettings.TextSettings, ts.CellSettings.TextSettings);
            }
            if (ts.HeaderSettings != null)
            {
                e.TableSettings.HeaderSettings.HasRepetitions = ts.HeaderSettings.HasRepetitions;
                if (ts.HeaderSettings.CellSettings != null)
                {
                    e.TableSettings.HeaderSettings.CellSettings.VerticalAlignment = ts.HeaderSettings.CellSettings.VerticalAlignment;
                    e.TableSettings.HeaderSettings.CellSettings.TopPadding = ts.HeaderSettings.CellSettings.TopPadding;
                    e.TableSettings.HeaderSettings.CellSettings.BottomPadding = ts.HeaderSettings.CellSettings.BottomPadding;
                    e.TableSettings.HeaderSettings.CellSettings.LeftPadding = ts.HeaderSettings.CellSettings.LeftPadding;
                    e.TableSettings.HeaderSettings.CellSettings.RightPadding = ts.HeaderSettings.CellSettings.RightPadding;
                    CopyText(e.TableSettings.HeaderSettings.CellSettings.TextSettings, ts.HeaderSettings.CellSettings.TextSettings);
                }
            }
        }

        var imgs = model.ImageSettings;
        if (imgs != null)
        {
            e.ImageSettings.LineSpacing = imgs.LineSpacing;
            e.ImageSettings.BeforeSpacing = imgs.BeforeSpacing;
            e.ImageSettings.AfterSpacing = imgs.AfterSpacing;
            e.ImageSettings.Justification = imgs.Justification;
            e.ImageSettings.Left = imgs.Left;
            e.ImageSettings.Right = imgs.Right;
            e.ImageSettings.FirstLine = imgs.FirstLine;
            e.ImageSettings.KeepWithNext = imgs.KeepWithNext;
            if (imgs.CaptionSettings != null)
            {
                e.ImageSettings.CaptionSettings.TextTemplate = imgs.CaptionSettings.TextTemplate;
                e.ImageSettings.CaptionSettings.Separator = imgs.CaptionSettings.Separator;
                e.ImageSettings.CaptionSettings.Position = imgs.CaptionSettings.Position;
                CopyText(e.ImageSettings.CaptionSettings.TextSettings, imgs.CaptionSettings.TextSettings);
            }
        }

        await _context.SaveChangesAsync();
        return e.Id;
    }

    // ─── Heading chain ────────────────────────────────────────────────────────

    private async Task UpdateChainAsync(FormattingTemplateModel existing, List<HeadingLevelEditItem>? items)
    {
        items ??= [];
        var old = FlattenChain(existing.HeadingSettings);
        int keep = Math.Min(old.Count, items.Count);

        for (int i = 0; i < keep; i++)
        {
            old[i].HeadingLevel = items[i].HeadingLevel;
            old[i].StartOnNewPage = items[i].StartOnNewPage;
            if (items[i].TextSettings != null) CopyText(old[i].TextSettings, items[i].TextSettings);
        }

        var prev = keep > 0 ? old[keep - 1] : null;
        for (int i = keep; i < items.Count; i++)
        {
            var node = new HeadingSettingsModel
            {
                HeadingLevel = items[i].HeadingLevel,
                StartOnNewPage = items[i].StartOnNewPage,
                TextSettings = items[i].TextSettings ?? new TextSettingsModel()
            };
            if (prev != null) prev.NextHeadingLevel = node;
            prev = node;
            old.Add(node);
        }

        if (items.Count > 0)
        {
            old[items.Count - 1].NextHeadingLevelId = null;
            old[items.Count - 1].NextHeadingLevel = null;
        }

        for (int i = items.Count; i < old.Count; i++)
        {
            RemoveIfNotNull(_context.TextSettings, old[i].TextSettings);
            _context.HeadingSettings.Remove(old[i]);
        }
    }

    private static List<HeadingSettingsModel> FlattenChain(HeadingSettingsModel? head)
    {
        var list = new List<HeadingSettingsModel>();
        var cur = head;
        while (cur != null) { list.Add(cur); cur = cur.NextHeadingLevel; }
        return list;
    }

    private static HeadingSettingsModel BuildChain(List<HeadingLevelEditItem>? items)
    {
        if (items == null || items.Count == 0)
            return new HeadingSettingsModel { HeadingLevel = 1, TextSettings = new TextSettingsModel() };

        HeadingSettingsModel? first = null, prev = null;
        foreach (var item in items.OrderBy(x => x.HeadingLevel))
        {
            var textSettings = item.TextSettings ?? new TextSettingsModel();
            textSettings.Id = 0;
            var node = new HeadingSettingsModel
            {
                Id = 0,
                HeadingLevel = item.HeadingLevel,
                StartOnNewPage = item.StartOnNewPage,
                TextSettings = textSettings
            };
            if (prev != null) prev.NextHeadingLevel = node;
            prev = node;
            first ??= node;
        }
        return first!;
    }

    // ─── Static helpers ───────────────────────────────────────────────────────

    private static TableCaptionSettingsModel ToTableCaption(ICaptionSettingsModel? src) => new()
    {
        TextTemplate = src?.TextTemplate ?? "",
        Separator = src?.Separator ?? " — ",
        Position = src?.Position ?? default,
        TextSettings = src?.TextSettings ?? new TextSettingsModel()
    };

    private static ImageCaptionSettingsModel ToImageCaption(ICaptionSettingsModel? src) => new()
    {
        TextTemplate = src?.TextTemplate ?? "",
        Separator = src?.Separator ?? " — ",
        Position = src?.Position ?? default,
        TextSettings = src?.TextSettings ?? new TextSettingsModel()
    };

    private static void CopyText(TextSettingsModel? dest, TextSettingsModel? src)
    {
        if (dest == null || src == null) return;
        dest.Font = src.Font;
        dest.FontSize = src.FontSize;
        dest.Color = src.Color;
        dest.IsBold = src.IsBold;
        dest.IsItalic = src.IsItalic;
        dest.IsUnderscore = src.IsUnderscore;
        dest.LineSpacingType = src.LineSpacingType;
        dest.LineSpacing = src.LineSpacing;
        dest.BeforeSpacing = src.BeforeSpacing;
        dest.AfterSpacing = src.AfterSpacing;
        dest.Justification = src.Justification;
        dest.Left = src.Left;
        dest.Right = src.Right;
        dest.FirstLine = src.FirstLine;
        dest.FirstLineType = src.FirstLineType;
        dest.KeepWithNext = src.KeepWithNext;
    }

    private static void CopyDoc(DocumentSettingsModel? dest, DocumentSettingsModel? src)
    {
        if (dest == null || src == null) return;
        dest.HasPageNumbers = src.HasPageNumbers;
        dest.HasTableCaptions = src.HasTableCaptions;
        dest.HasImageCaptions = src.HasImageCaptions;
    }

    private static void RemoveIfNotNull<T>(DbSet<T> set, T? entity) where T : class
    {
        if (entity != null) set.Remove(entity);
    }
}
