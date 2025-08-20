using idat_bank.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de DbContext para utilizar la cadena de conexión desde appsettings.json
builder.Services.AddDbContext<IdatBankContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BancoDBConn")));  // Usar la cadena de conexión definida en appsettings.json

// Registrar los controladores con vistas
builder.Services.AddControllersWithViews();

// Configuración de CORS (según lo que configuraste previamente)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://idat-bank.netlify.app")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Usar CORS
app.UseCors("AllowFrontend");

// Configuración para la ruta y autorización
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.Run();
