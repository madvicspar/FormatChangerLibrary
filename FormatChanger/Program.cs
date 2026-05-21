using FormatChanger.Infrastructure.Email;
using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;
using FormatChanger.Services;
using FormatChanger.Services.Interfaces;
using FormatChanger.Services.Strategies;
using FormatChanger.Utilities.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
    options.ModelBinderProviders.Insert(0, new FormatChanger.Infrastructure.ModelBinders.FloatModelBinderProvider()));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IEmailSenderCustom, EmailSender>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<IElementCorrectionStrategy<TextSettingsModel>, TextCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<HeadingSettingsModel>, HeadingFirstCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<ImageSettingsModel>, ImageCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<ImageCaptionSettingsModel>, ImageCaptionCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<TableCaptionSettingsModel>, TableCaptionCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<TableSettingsModel>, TableCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<CellSettingsModel>, TableCellCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<HeaderSettingsModel>, TableHeaderCorrectionStrategy>();
builder.Services.AddScoped<IDocumentStorage, DocumentStorageService>();
builder.Services.AddScoped<IParagraphExtractor, ParagraphExtractor>();
builder.Services.AddScoped<IDocumentCorrector, DocumentCorrector>();
builder.Services.AddScoped<IDocumentChecker, DocumentChecker>();
builder.Services.AddScoped<IParagraphStyler, ParagraphStyler>();
builder.Services.AddScoped<IParagraphNumbering, ParagraphNumbering>();


string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options
                    => options.UseNpgsql(connectionString));
builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();
builder.Services.AddSession();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await dbContext.ClearAndSeed(dbContext, scope.ServiceProvider, userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();