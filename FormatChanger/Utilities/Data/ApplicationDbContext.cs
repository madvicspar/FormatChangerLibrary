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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ICaptionSettingsModel>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<ImageCaptionSettingsModel>("Image")
                .HasValue<TableCaptionSettingsModel>("Table");
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
        public DbSet<ICaptionSettingsModel> CaptionSettings { get; set; } = null!;
        public DbSet<ImageSettingsModel> ImageSettings { get; set; } = null!;
        public DbSet<TableSettingsModel> TableSettings { get; set; } = null!;
        public DbSet<CellSettingsModel> CellSettings { get; set; } = null!;
        public DbSet<HeaderSettingsModel> HeaderSettings { get; set; } = null!;
        public DbSet<DocumentSettingsModel> DocumentSettings { get; set; } = null!;
        public void ClearAndSeed(ApplicationDbContext _context)
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            SeedData(_context);
        }
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            var testUser = new UserModel
            {
                UserName = "me_test",
                TelegramUserName = "madvicspar"
            };

            var user = await userManager.FindByNameAsync(testUser.UserName);
            if (user == null)
            {
                var createUserResult = await userManager.CreateAsync(testUser, "Test!123");

                if (createUserResult.Succeeded)
                {
                    var roleExist = await roleManager.RoleExistsAsync("UserRole");
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole("UserRole"));
                    }

                    await userManager.AddToRoleAsync(testUser, "UserRole");
                }
            }
        }
        public void SeedData(ApplicationDbContext context)
        {
            // Шаблон форматирования для РИС-22
            // **1. Данные для настроек обычного текста**
            var textSettings = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000000",
                IsBold = false,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 13,
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

            // **2. Данные для настроек текста - основы заголовка первого уровня**
            var textSettings_h1 = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000000",
                IsBold = true,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 16,
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

            // **3. Данные для настроек текста - основы заголовка второго уровня**
            var textSettings_h2 = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000000",
                IsBold = true,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 14,
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

            // **4. Данные для настроек текста - основы заголовка третьего уровня**
            var textSettings_h3 = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000000",
                IsBold = true,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 13,
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

            // **5. Данные для настроек текста - основы подписи к таблице**
            var textSettings_tableCaption = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000000",
                IsBold = false,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 13,
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

            // **6. Данные для настроек текста - основы подписи к изображению**
            var textSettings_imageCaption = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000000",
                IsBold = true,
                IsItalic = true,
                IsUnderscore = false,
                FontSize = 11,
                LineSpacing = 240,
                BeforeSpacing = 0,
                AfterSpacing = 120,
                Justification = "Center",
                Left = 0,
                Right = 0,
                FirstLine = 0,
                KeepWithNext = false
            };

            context.TextSettings.Add(textSettings_imageCaption);
            context.SaveChanges();

            // **7. Данные для настроек текста - основы ячеек таблицы**
            var textSettings_cells = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000000",
                IsBold = false,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 11,
                LineSpacing = 240,
                BeforeSpacing = 0,
                AfterSpacing = 0,
                Justification = "Left",
                Left = 0,
                Right = 0,
                FirstLine = 0,
                KeepWithNext = false
            };

            context.TextSettings.Add(textSettings_cells);
            context.SaveChanges();

            // **7. Данные для настроек текста - основы ячеек таблицы для заголовков**
            var textSettings_cells_header = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000000",
                IsBold = false,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 11,
                LineSpacing = 240,
                BeforeSpacing = 0,
                AfterSpacing = 0,
                Justification = "Center",
                Left = 0,
                Right = 0,
                FirstLine = 0,
                KeepWithNext = true
            };

            context.TextSettings.Add(textSettings_cells_header);
            context.SaveChanges();

            // **1. Данные для настроек текста - основы списков**
            var textSettings_list = new TextSettingsModel
            {
                Font = "Times New Roman",
                Color = "000000",
                IsBold = false,
                IsItalic = false,
                IsUnderscore = false,
                FontSize = 13,
                LineSpacing = 360,
                BeforeSpacing = 0,
                AfterSpacing = 0,
                Justification = "Both",
                Left = 1.5f,
                Right = 0.5f,
                FirstLine = 1.25f,
                KeepWithNext = false
            };

            context.TextSettings.Add(textSettings_list);
            context.SaveChanges();

            // **. Данные для настроек заголовка первого уровня**
            var headingSettings1 = new HeadingSettingsModel
            {
                TextSettings = textSettings_h1,
                HeadingLevel = 1,
                StartOnNewPage = true
            };

            context.HeadingSettings.Add(headingSettings1);
            context.SaveChanges();

            // **. Данные для настроек заголовка второго уровня**
            var headingSettings2 = new HeadingSettingsModel
            {
                TextSettings = textSettings_h2,
                HeadingLevel = 2,
                StartOnNewPage = true
            };

            context.HeadingSettings.Add(headingSettings2);
            context.SaveChanges();

            // **. Данные для настроек заголовка третьего уровня**
            var headingSettings3 = new HeadingSettingsModel
            {
                TextSettings = textSettings_h3,
                HeadingLevel = 3,
                StartOnNewPage = true
            };

            context.HeadingSettings.Add(headingSettings3);
            context.SaveChanges();

            // **. Данные для настроек подписи к таблице**
            var tableCaptionSettings = new TableCaptionSettingsModel
            {
                TextSettings = textSettings_tableCaption,
                TextTemplate = "Таблица\\s+\\d+\\s+-\\s+(.+)"
            };

            context.CaptionSettings.Add(tableCaptionSettings);
            context.SaveChanges();

            // **. Данные для настроек подписи к рисунку**
            var imageCaptionSettings = new ImageCaptionSettingsModel
            {
                TextSettings = textSettings_imageCaption,
                TextTemplate = "Рисунок\\s+\\d+\\s+-\\s+(.+)"
            };

            context.CaptionSettings.Add(imageCaptionSettings);
            context.SaveChanges();

            // **. Данные для настроек рисунка**
            var imageSettings = new ImageSettingsModel
            {
                CaptionSettings = imageCaptionSettings,
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

            // **. Данные для настроек ячеек таблицы**
            // значения полей неверные
            var cellSettings = new CellSettingsModel
            {
                TextSettings = textSettings_cells,
                VerticalAlignment = "Top",
                TopPadding = 2,
                LeftPadding = 2,
                BottomPadding = 2,
                RightPadding = 2
            };

            context.CellSettings.Add(cellSettings);
            context.SaveChanges();

            // **. Данные для настроек ячеек таблицы для заголовка**
            // значения полей неверные
            var cellSettings_header = new CellSettingsModel
            {
                TextSettings = textSettings_cells_header,
                VerticalAlignment = "Top",
                TopPadding = 2,
                LeftPadding = 2,
                BottomPadding = 2,
                RightPadding = 2
            };

            context.CellSettings.Add(cellSettings_header);
            context.SaveChanges();

            var headerSettings = new HeaderSettingsModel
            {
                CellSettings = cellSettings_header,
                HasRepetitions = true
            };

            context.HeaderSettings.Add(headerSettings);
            context.SaveChanges();

            var tableSettings = new TableSettingsModel
            {
                BeforeSpacing = 0,
                AfterSpacing = 120,
                CaptionSettings = tableCaptionSettings,
                CellSettings = cellSettings,
                HeaderSettings = headerSettings
            };

            context.TableSettings.Add(tableSettings);
            context.SaveChanges();

            var documentSettings = new DocumentSettingsModel
            {
                HasPageNumbers = true,
                HasTableCaptions = true,
                HasImageCaptions = true
            };

            context.DocumentSettings.Add(documentSettings);
            context.SaveChanges();

            // маркированный список просто
            var listSettings = new ListSettingsModel
            {
                EndType = Ends.Semicolon,
                IsNumeric = false,
                MarkerType = "-",
                ListLevel = 0,
                TextSettings = textSettings_list
            };

            context.ListSettings.Add(listSettings);
            context.SaveChanges();

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