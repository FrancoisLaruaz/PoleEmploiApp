using Microsoft.EntityFrameworkCore;
using PoleEmploiApp.DataEntities;
using PoleEmploiApp.DataEntities.Repositories.Interfaces;
using PoleEmploiApp.Services;
using PoleEmploiApp.Services.Interfaces;
using PoleEmploiApp.Services.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IJobOfferService, JobOfferService>();
builder.Services.AddScoped<IPoleEmploiAPIService, PoleEmploiAPIService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


var settings = builder.Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();

var DefaultConnection = settings.DefaultConnection;

builder.Services.AddDbContext<PoleEmploiContext>(options => options.UseSqlServer(DefaultConnection));

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
