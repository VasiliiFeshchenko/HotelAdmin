using HotelAdmin.Services.IsProductionChecker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcTest.Controllers.Filters;
using MvcTest.Data;
using MvcTest.Models;
using OfficeOpenXml;
using System;

var builder = WebApplication.CreateBuilder(args);

// Connection to the database
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .Build();

string connectionString = configuration.GetConnectionString("Main");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 32));
builder.Services.AddDbContext<MvcTestContext>(options =>
    options.UseMySql(connectionString, serverVersion));

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<HotelFilter>();
builder.Services.AddScoped<AuthenticationFilter>();
builder.Services.AddHttpContextAccessor(); // Add this line to register IHttpContextAccessor

// Make the program search for views in specific paths
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Add("/Views/AdministratorsViews/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/ClientsViews/{1}/{0}.cshtml");
    });
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
});

var app = builder.Build();

// Configure the HTTP request pipeline
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

app.UseSession();

string pattern = AppState.IsProduction ?
    "{controller=BookingOptions}/{action=BookingOptionsParameters}/{hotelId=1}" :  //Production

    "{controller=Orders}/{action=Index}/{hotelId=1}";              //Development

app.MapControllerRoute(
    name: "default",
    pattern: pattern); //When setting default hotelId also change Filters.HotelFilter
//   {controller=RoomsReservationsTable}/{action=Index}/{hotelId=1}
//   {controller=BookingOptions}/{action=BookingOptionsParameters}/{hotelId=1}
//   {controller=ClientsRoomsReservationsTable}/{action=Index}
//   {controller=BookableObjectReservation}/{action=Index}
app.Run();



