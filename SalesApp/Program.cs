using Microsoft.EntityFrameworkCore;
using SalesApp.Models;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Set license untuk EPPlus
//ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SalesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SalesDB")));

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
