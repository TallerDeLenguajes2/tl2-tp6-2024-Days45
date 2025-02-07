using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EspacioTp5;
using rapositoriosTP5;
using System;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddHttpContextAccessor();

var CadenaDeConexion = builder.Configuration.GetConnectionString("SqliteConexion")!;
builder.Services.AddSingleton<string>(CadenaDeConexion);
builder.Services.AddLogging();
builder.Services.AddSingleton<IProductoRepository, ProductoRepository>(provider =>
    new ProductoRepository(
        CadenaDeConexion,
        provider.GetRequiredService<ILogger<ProductoRepository>>()
    ));

builder.Services.AddSingleton<IPresupuestoRepository, PresupuestoRepository>(provider =>
    new PresupuestoRepository(
        CadenaDeConexion,
        provider.GetRequiredService<ILogger<PresupuestoRepository>>(),
        provider.GetRequiredService<IProductoRepository>(),
        provider.GetRequiredService<IClienteRepository>()
    ));

builder.Services.AddSingleton<IUsuariosRepository, RepositorioUsuariosSqlite>(provider =>
    new RepositorioUsuariosSqlite(
        CadenaDeConexion,
        provider.GetRequiredService<ILogger<RepositorioUsuariosSqlite>>()
    ));

builder.Services.AddSingleton<IClienteRepository, ClienteRepository>(provider =>
    new ClienteRepository(
        CadenaDeConexion,
        provider.GetRequiredService<ILogger<ClienteRepository>>()
    ));


builder.Services.AddControllersWithViews();


var app = builder.Build();

app.UseSession();


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
    pattern: "{controller=Login}/{action=Index}/{id?}");


app.Run();
