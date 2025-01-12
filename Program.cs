using Microsoft.AspNetCore.Authentication;
using EspacioTp5;
using rapositoriosTP5;

var builder = WebApplication.CreateBuilder(args);

// Habilitar servicios de sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});

// Acceso al contexto HTTP
builder.Services.AddHttpContextAccessor();

// Registrar repositorios personalizados
builder.Services.AddSingleton<IProductoRepository, ProductoRepository>();
builder.Services.AddSingleton<IPresupuestoRepository, PresupuestoRepository>();
builder.Services.AddScoped<IUsuariosRepository, RepositorioUsuariosSqlite>();

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
