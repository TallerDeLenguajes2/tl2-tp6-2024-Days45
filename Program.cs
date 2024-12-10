using EspacioTp5; // Espacio de nombres donde están las clases del modelo
using rapositoriosTP5; // Espacio de nombres donde están los repositorios

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar repositorios para inyección de dependencias
builder.Services.AddSingleton<IProductoRepository, ProductoRepository>();
builder.Services.AddSingleton<IPresupuestoRepository, PresupuestoRepository>();
builder.Services.AddSingleton<IUsuariosRepository, UsuariosRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Configuración para HSTS
}

app.UseHttpsRedirection(); // Forzar redirección a HTTPS
app.UseStaticFiles(); // Habilitar archivos estáticos

app.UseRouting(); // Habilitar routing
app.UseAuthorization(); // Middleware de autorización

// Configurar ruta predeterminada
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Productos}/{action=Index}/{id?}");

// Ejecutar la aplicación
app.Run();
