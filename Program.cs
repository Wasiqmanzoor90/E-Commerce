using E_Commerce.Data;
using E_Commerce.Interface;
using E_Commerce.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session support
builder.Services.AddSession();

// Register IHttpContextAccessor
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Database Context
builder.Services.AddDbContext<SqldbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DataBase")));

builder.Services.AddScoped<IJasonToken, JasonTokenServicecs>();
builder.Services.AddScoped<ICloudinaryInterface, CloudnaryService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession(); // ✅ Session middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();



app.UseMiddleware<TokenMiddleware>(); // ✅ Move TokenMiddleware after authentication

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
