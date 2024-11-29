using AgricolaDH_GApp.Controllers;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Register DbContext
var connectionString = builder.Configuration.GetConnectionString("DbConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

//Services
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ProveedorService>();
builder.Services.AddScoped<AreaService>();
builder.Services.AddScoped<RolService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<CultivoService>();
builder.Services.AddScoped<RanchoService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<RazorViewEngine>();
builder.Services.AddScoped<HttpContextAccessor>();
builder.Services.AddScoped<ViewRenderService>();

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

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
