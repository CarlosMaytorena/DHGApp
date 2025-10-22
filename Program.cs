using AgricolaDH_GApp.Controllers;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Helper;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Register DbContext
var connectionString = builder.Configuration.GetConnectionString("DbConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Register BlobStorageService for Egresos
var connectionStringBlob = builder.Configuration.GetConnectionString("AzureBlobStorage");
builder.Services.AddSingleton<BlobStorageService>(new BlobStorageService(connectionStringBlob, "evidencias"));


//Services
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ProveedorService>();
builder.Services.AddScoped<AreaService>();
builder.Services.AddScoped<RolService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<CultivoService>();
builder.Services.AddScoped<RanchoService>();
builder.Services.AddScoped<EtapaService>();
builder.Services.AddScoped<TemporadaService>();
builder.Services.AddScoped<OrdenDeCompraService>();
builder.Services.AddScoped<AlmacenService>();
builder.Services.AddScoped<EgresoService>();
builder.Services.AddScoped<ConstanteService>();

builder.Services.AddScoped<SerialMapService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<RazorViewEngine>();
builder.Services.AddScoped<HttpContextAccessor>();
builder.Services.AddScoped<ViewRenderService>();

builder.Services.AddSingleton<Email>();
builder.Services.AddSession();
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Login/Index"; // Change if your login URL is different
        options.AccessDeniedPath = "/Login/Index"; // opcional, cuando no tiene permisos

    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
