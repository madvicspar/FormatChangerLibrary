using FormatChanger.Models;
using FormatChanger.Services;
using FormatChanger.Utilities.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IElementCorrectionStrategy<TextSettingsModel>, TextCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<HeadingSettingsModel>, HeadingFirstCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<ImageSettingsModel>, ImageCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<ImageCaptionSettingsModel>, ImageCaptionCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<TableCaptionSettingsModel>, TableCaptionCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<TableSettingsModel>, TableCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<CellSettingsModel>, TableCellCorrectionStrategy>();
builder.Services.AddScoped<IElementCorrectionStrategy<HeaderSettingsModel>, TableHeaderCorrectionStrategy>();


string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options
                    => options.UseNpgsql(connectionString));
builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    //dbContext.ClearAndSeed(dbContext);
    //await ApplicationDbContext.Initialize(scope.ServiceProvider, userManager, roleManager);
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
