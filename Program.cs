using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using ApiLb3.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001", "http://*:5000", "https://*:5001");
var configuration = builder.Configuration;
var connectionString = builder.Configuration.GetConnectionString("sql");

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
});
builder.Services.AddDbContext<DataContext>(options => options.UseMySQL(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuarios/Login";
        options.LogoutPath = "/Usuarios/Logout";
        options.AccessDeniedPath = "/Home/Restringido";
        // options.ExpireTimeSpan = TimeSpan.FromMinutes(5); // Tiempo de expiración
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["TokenAuthentication:Issuer"],
            ValidAudience = configuration["TokenAuthentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(configuration["TokenAuthentication:SecretKey"])),
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/chatsegurohub") ||
                     path.StartsWithSegments("/api/propietarios/reset") ||
                     path.StartsWithSegments("/api/propietarios/token")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    // options.AddPolicy("Empleado", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador", "Empleado"));
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador", "SuperAdministrador"));
});
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddMvc();
builder.Services.AddSignalR();

Console.WriteLine("Successfully connected to the database!");

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseStaticFiles();
app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
});

// Habilitar autenticación
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute("login", "entrar/{**accion}", new { controller = "Usuarios", action = "Login" });
app.MapControllerRoute("rutaFija", "ruteo/{valor}", new { controller = "Home", action = "Ruta", valor = "defecto" });
app.MapControllerRoute("fechas", "{controller=Home}/{action=Fecha}/{anio}/{mes}/{dia}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();
