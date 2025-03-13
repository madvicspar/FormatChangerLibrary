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


string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options
                    => options.UseNpgsql(connectionString));
//builder.Services.AddIdentity<UserModel, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

var app = builder.Build();


//// Используем инициализацию данных после запуска приложения
//using (var scope = app.Services.CreateScope())
//{
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//    var testUser = new UserModel
//    {
//        UserName = "me_test",
//        TelegramUserName = "madvicspar"
//    };

//    // Проверяем, существует ли уже пользователь, и если нет - создаем его
//    var user = await userManager.FindByNameAsync(testUser.UserName);
//    if (user == null)
//    {
//        var createUserResult = await userManager.CreateAsync(testUser, "Test!123");
//    }
//}


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
