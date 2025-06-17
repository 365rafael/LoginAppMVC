using LoginAppMVC.Data;
using LoginAppMVC.Helpers;
using LoginAppMVC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Autenticação por cookie
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Conta/Login";
        options.LogoutPath = "/Conta/Logout";
    });

// serviçode email
builder.Services.AddScoped<EmailService>();


builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Antes do Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope()) // criar usuário teste
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Usuario.Any())
    {
        db.Usuario.Add(new LoginAppMVC.Models.Usuario
        {
            Nome = "Admin",
            Email = "admin@email.com",
            Senha = PasswordHelper.HashPassword("admin") //  Senha criptografada
        });
        db.SaveChanges();
    }
}


app.Run();

