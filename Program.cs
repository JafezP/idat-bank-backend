using idat_bank.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de DbContext para utilizar la cadena de conexi�n desde appsettings.json
builder.Services.AddDbContext<IdatBankContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BancoDBConn")));  // Usar la cadena de conexi�n definida en appsettings.json

// Registrar los controladores con vistas
builder.Services.AddControllersWithViews();

// Configuraci�n de CORS (seg�n lo que configuraste previamente)
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

// Configuraci�n para la ruta y autorizaci�n
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.Run();
