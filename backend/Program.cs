using FormatChanger.WebAPI.Infrastructure.Data;
using FormatChanger.WebAPI.Models;
using FormatChanger.WebAPI.Models.FormattingModels;
using FormatChanger.WebAPI.Services;
using FormatChanger.WebAPI.Services.Interfaces;
using FormatChanger.WebAPI.Services.Strategies;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddSingleton<IEmailSenderCustom, EmailSender>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
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

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ??
	builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options
					=> options.UseNpgsql(connectionString));
builder.Services.AddIdentity<UserModel, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
	options.AddPolicy("Frontend", policy =>
	{
		policy.WithOrigins("http://localhost") // для локального теста, в продакшене заменить на домен фронта
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

var app = builder.Build();

// Применение миграций при запуске
using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	dbContext.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	await dbContext.ClearAndSeed(dbContext, scope.ServiceProvider, userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
