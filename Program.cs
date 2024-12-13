using Microsoft.AspNetCore.Authentication;
using EspacioTp5;
using rapositoriosTP5;

var builder = WebApplication.CreateBuilder(args);

// Habilitar servicios de sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true; // Solo accesible desde HTTP, no JavaScript
    options.Cookie.IsEssential = true; // Necesario incluso si el usuario no acepta cookies
});

// Acceso al contexto HTTP
builder.Services.AddHttpContextAccessor();

// Registrar repositorios personalizados
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IPresupuestoRepository, PresupuestoRepository>();
builder.Services.AddScoped<IUsuariosRepository, RepositorioUsuariosEnMemoria>();
// Servicios de autenticación personalizados (si corresponde implementarlos)
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Agregar servicios de controladores y vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Usar sesiones
app.UseSession();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

// Ejecutar la aplicación
app.Run();
