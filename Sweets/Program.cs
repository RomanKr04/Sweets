using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Sweets.Models;

var builder = WebApplication.CreateBuilder(args);

// ��������� ������ ����������� �� ������������
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ����������� ��������� ���� ������
builder.Services.AddDbContext<SweetContext>(options =>
    options.UseNpgsql(connectionString));

// ��������� �������������� � �������������� cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// ���������� �������� �����������
builder.Services.AddAuthorization();

// ���������� MVC-������������ � ���������� ������������
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
});

var app = builder.Build();

// ������������ ��������� HTTP-��������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ����������� middleware ��� �������������� � �����������
app.UseAuthentication(); // ������ ���� ����� UseAuthorization
app.UseAuthorization();

// ��������� ��������� �� ���������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
