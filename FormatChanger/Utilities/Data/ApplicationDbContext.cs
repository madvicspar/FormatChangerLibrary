using FormatChanger.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FormatChanger.Utilities.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<DocumentModel> Documents { get; set; } = null!;
        public DbSet<FormattingTemplateModel> FormattingTemplates { get; set; } = null!;
        public DbSet<EvaluationSystemModel> EvaluationSystems { get; set; } = null!;
        public DbSet<CorrectionResultsModel> CorrectionResults { get; set; } = null!;
        public DbSet<EvaluationResultsModel> EvaluationResults { get; set; } = null!;
        public DbSet<TextSettingsModel> TextSettings { get; set; } = null!;
        public DbSet<HeadingSettingsModel> HeadingSettings { get; set; } = null!;
        public DbSet<ListSettingsModel> ListSettings { get; set; } = null!;
        public DbSet<CaptionSettingsModel> CaptionSettings { get; set; } = null!;
        public DbSet<ImageSettingsModel> ImageSettings { get; set; } = null!;
        public DbSet<TableSettingsModel> TableSettings { get; set; } = null!;
        public DbSet<CellSettingsModel> CellSettings { get; set; } = null!;
        public DbSet<HeaderSettingsModel> HeaderSettings { get; set; } = null!;
        public DbSet<DocumentSettingsModel> DocumentSettings { get; set; } = null!;
        protected void SeedData(ApplicationDbContext context)
        {
            // Шаблон форматирования для РИС-22
            // **1. Данные для настроек обычного текста**
            var textSettings = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000",
                IsBold = false,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 26,
                LineSpacing = 360,
                BeforeSpacing = 0,
                AfterSpacing = 0,
                Justification = "Both",
                Left = 0,
                Right = 0,
                FirstLine = 1.25f,
                KeepWithNext = false
            };

            context.TextSettings.Add(textSettings);
            context.SaveChanges();

            var textSettingsId = context.TextSettings.Last().Id;

            // **2. Данные для настроек текста - основы заголовка первого уровня**
            var textSettings_h1 = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000",
                IsBold = true,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 32,
                LineSpacing = 240,
                BeforeSpacing = 0,
                AfterSpacing = 240,
                Justification = "Center",
                Left = 0,
                Right = 0,
                FirstLine = 0,
                KeepWithNext = true
            };

            context.TextSettings.Add(textSettings_h1);
            context.SaveChanges();

            var textSettings_h1Id = context.TextSettings.Last().Id;

            // **3. Данные для настроек текста - основы заголовка второго уровня**
            var textSettings_h2 = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000",
                IsBold = true,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 28,
                LineSpacing = 240,
                BeforeSpacing = 240,
                AfterSpacing = 120,
                Justification = "Center",
                Left = 0,
                Right = 0,
                FirstLine = 0,
                KeepWithNext = true
            };

            context.TextSettings.Add(textSettings_h2);
            context.SaveChanges();

            var textSettings_h2Id = context.TextSettings.Last().Id;

            // **4. Данные для настроек текста - основы заголовка третьего уровня**
            var textSettings_h3 = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000",
                IsBold = true,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 26,
                LineSpacing = 240,
                BeforeSpacing = 160,
                AfterSpacing = 80,
                Justification = "Center",
                Left = 0,
                Right = 0,
                FirstLine = 0,
                KeepWithNext = true
            };

            context.TextSettings.Add(textSettings_h3);
            context.SaveChanges();

            var textSettings_h3Id = context.TextSettings.Last().Id;

            // **5. Данные для настроек текста - основы подписи к таблице**
            var textSettings_tableCaption = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000",
                IsBold = false,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 26,
                LineSpacing = 240,
                BeforeSpacing = 120,
                AfterSpacing = 0,
                Justification = "Both",
                Left = 0,
                Right = 0,
                FirstLine = 0,
                KeepWithNext = true
            };

            context.TextSettings.Add(textSettings_tableCaption);
            context.SaveChanges();

            var textSettings_tableCaptionId = context.TextSettings.Last().Id;

            // **6. Данные для настроек текста - основы подписи к изображению**
            var textSettings_imageCaption = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000",
                IsBold = true,
                IsItalic = true,
                IsUnderscore = false,
                FontSize = 22,
                LineSpacing = 240,
                BeforeSpacing = 0,
                AfterSpacing = 120,
                Justification = "Both",
                Left = 0,
                Right = 0,
                FirstLine = 0,
                KeepWithNext = true
            };

            context.TextSettings.Add(textSettings_imageCaption);
            context.SaveChanges();

            var textSettings_imageCaptionId = context.TextSettings.Last().Id;

            // **7. Данные для настроек текста - основы ячеек таблицы**
            var textSettings_cells = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000",
                IsBold = false,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 22,
                LineSpacing = 240,
                BeforeSpacing = 0,
                AfterSpacing = 0,
                Justification = "Both",
                Left = 0,
                Right = 0,
                FirstLine = 0,
                KeepWithNext = false
            };

            context.TextSettings.Add(textSettings_cells);
            context.SaveChanges();

            var textSettings_cellsId = context.TextSettings.Last().Id;

            // **7. Данные для настроек текста - основы ячеек таблицы для заголовков**
            var textSettings_cells_header = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000",
                IsBold = true,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 22,
                LineSpacing = 240,
                BeforeSpacing = 0,
                AfterSpacing = 0,
                Justification = "Both",
                Left = 0,
                Right = 0,
                FirstLine = 0,
                KeepWithNext = false
            };

            context.TextSettings.Add(textSettings_cells_header);
            context.SaveChanges();

            var textSettings_cells_headerId = context.TextSettings.Last().Id;

            // **1. Данные для настроек текста - основы списков**
            var textSettings_list = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000",
                IsBold = false,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 26,
                LineSpacing = 360,
                BeforeSpacing = 0,
                AfterSpacing = 0,
                Justification = "Both",
                Left = 1.5f,
                Right = 0.5f,
                FirstLine = 1.25f,
                KeepWithNext = true
            };

            context.TextSettings.Add(textSettings_list);
            context.SaveChanges();

            var textSettings_listId = context.TextSettings.Last().Id;

            // **. Данные для настроек заголовка первого уровня**
            var headingSettings1 = new HeadingSettingsModel
            {
                TextSettingsId = textSettings_h1Id,
                HeadingLevel = 1,
                StartOnNewPage = true
            };

            context.HeadingSettings.Add(headingSettings1);
            context.SaveChanges();

            var headingSettings1Id = context.HeadingSettings.Last().Id;

            // **. Данные для настроек заголовка второго уровня**
            var headingSettings2 = new HeadingSettingsModel
            {
                TextSettingsId = textSettings_h2Id,
                HeadingLevel = 2,
                StartOnNewPage = true
            };

            context.HeadingSettings.Add(headingSettings2);
            context.SaveChanges();

            var headingSettings2Id = context.HeadingSettings.Last().Id;

            // **. Данные для настроек заголовка третьего уровня**
            var headingSettings3 = new HeadingSettingsModel
            {
                TextSettingsId = textSettings_h3Id,
                HeadingLevel = 3,
                StartOnNewPage = true
            };

            context.HeadingSettings.Add(headingSettings3);
            context.SaveChanges();

            var headingSettings3Id = context.HeadingSettings.Last().Id;

            // **. Данные для настроек подписи к рисунку**
            var tableCaptionSettings = new CaptionSettingsModel
            {
                TextSettingsId = textSettings_tableCaptionId,
                TextTemplate = "Таблица\\s+\\d+\\s+-\\s+(.+)"
            };

            context.CaptionSettings.Add(tableCaptionSettings);
            context.SaveChanges();

            var tableCaptionSettingsId = context.CaptionSettings.Last().Id;

            // **. Данные для настроек подписи к рисунку**
            var imageCaptionSettings = new CaptionSettingsModel
            {
                TextSettingsId = textSettings_imageCaptionId,
                TextTemplate = "Рисунок\\s+\\d+\\s+-\\s+(.+)"
            };

            context.CaptionSettings.Add(imageCaptionSettings);
            context.SaveChanges();

            var imageCaptionSettingsId = context.CaptionSettings.Last().Id;

            // **. Данные для настроек рисунка**
            var imageSettings = new ImageSettingsModel
            {
                CaptionSettingsId = imageCaptionSettingsId,
                LineSpacing = 240,
                BeforeSpacing = 120,
                AfterSpacing = 0,
                Justification = "Center",
                Left = 0,
                Right = 0,
                FirstLine = 0,
                KeepWithNext = true
            };

            context.ImageSettings.Add(imageSettings);
            context.SaveChanges();

            var imageSettingsId = context.ImageSettings.Last().Id;

            // **. Данные для настроек ячеек таблицы**
            // значения полей неверные
            var cellSettings = new CellSettingsModel
            {
                TextSettingsId = textSettings_cellsId,
                VerticalAlignment = "Top",
                TopPadding = 0,
                LeftPadding = 0,
                BottomPadding = 0,
                RightPadding = 0
            };

            context.CellSettings.Add(cellSettings);
            context.SaveChanges();

            var cellSettingsId = context.CellSettings.Last().Id;

            // **. Данные для настроек ячеек таблицы для заголовка**
            // значения полей неверные
            var cellSettings_header = new CellSettingsModel
            {
                TextSettingsId = textSettings_cells_headerId,
                VerticalAlignment = "Top",
                TopPadding = 0,
                LeftPadding = 0,
                BottomPadding = 0,
                RightPadding = 0
            };

            context.CellSettings.Add(cellSettings_header);
            context.SaveChanges();

            var cellSettings_headerId = context.CellSettings.Last().Id;

            var headerSettings = new HeaderSettingsModel
            {
                CellSettingsId = cellSettings_headerId,
                HasRepetitions = true
            };

            context.HeaderSettings.Add(headerSettings);
            context.SaveChanges();

            var headerSettingsId = context.HeaderSettings.Last().Id;

            var tableSettings = new TableSettingsModel
            {
                BeforeSpacing = 0,
                AfterSpacing = 120,
                CaptionSettingsId = tableCaptionSettingsId,
                CellSettingsId = cellSettingsId,
                HeaderSettingsModelId = headerSettingsId,
            };

            context.TableSettings.Add(tableSettings);
            context.SaveChanges();

            var tableSettingsId = context.TableSettings.Last().Id;

            var documentSettings = new DocumentSettingsModel
            {
                HasPageNumbers = true,
                HasTableCaptions = true,
                HasImageCaptions = true
            };

            context.DocumentSettings.Add(documentSettings);
            context.SaveChanges();

            var documentSettingsId = context.DocumentSettings.Last().Id;

            // маркированный список просто
            var listSettings = new ListSettingsModel
            {
                EndType = Ends.Semicolon,
                IsNumeric = false,
                MarkerType = "-",
                ListLevel = 0,
                TextSettingsId = textSettings_listId
            };

            context.ListSettings.Add(listSettings);
            context.SaveChanges();

            var listSettingsId = context.ListSettings.Last().Id;

            // TODO: пофиксить логику шаблона - заголовки например вплоть до 8 уровня, списки - хотя бы три варианта
            // **3. Данные для шаблона форматирования**
            var formattingTemplate = new FormattingTemplateModel
            {
                Title = "HSE RIS-22",
                TextSettings = textSettings,
                TableSettings = tableSettings,
                HeadingSettings = headingSettings1,
                ListSettings = listSettings,
                ImageSettings = imageSettings,
                DocumentSettings = documentSettings
            };

            context.FormattingTemplates.Add(formattingTemplate);
            context.SaveChanges();
        }
    }
}