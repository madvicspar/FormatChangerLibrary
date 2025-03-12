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
        public DbSet<ListSettingsModel> ListSettings { get; set; } = null!;
        public DbSet<CaptionSettingsModel> CaptionSettings { get; set; } = null!;
        public DbSet<ImageSettingsModel> ImageSettings { get; set; } = null!;
        public DbSet<TableSettingsModel> TableSettings { get; set; } = null!;
        public DbSet<CellSettingsModel> CellSettings { get; set; } = null!;
        public DbSet<HeaderSettingsModel> HeaderSettings { get; set; } = null!;
        public DbSet<DocumentSettingsModel> DocumentSettings { get; set; } = null!;
    }
}
