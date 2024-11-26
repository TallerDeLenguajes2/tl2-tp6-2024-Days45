using tl2_tp6_2024_Days45.Controllers;
using rapositoriosTP5; // Asegúrate de que este espacio de nombres sea correcto

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar el repositorio
builder.Services.AddScoped<IClienteRepository, ClienteRepository>(); // Asegúrate de que ClienteRepository implemente IClienteRepository

builder.Services.AddScoped<PresupuestosController>(provider =>
    new PresupuestosController(provider.GetRequiredService<ILogger<PresupuestosController>>()));

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Presupuestos}/{action=Index}/{id?}");

app.Run();