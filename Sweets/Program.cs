using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Sweets.Models;

var builder = WebApplication.CreateBuilder(args);

// Получение строки подключения из конфигурации
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Регистрация контекста базы данных
builder.Services.AddDbContext<SweetContext>(options =>
    options.UseNpgsql(connectionString));

// Настройка аутентификации с использованием cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Добавление политики авторизации
builder.Services.AddAuthorization();

// Добавление MVC-контроллеров с глобальной авторизацией
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
});

var app = builder.Build();

// Конфигурация пайплайна HTTP-запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Подключение middleware для аутентификации и авторизации
app.UseAuthentication(); // Должно быть перед UseAuthorization
app.UseAuthorization();

// Настройка маршрутов по умолчанию
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
