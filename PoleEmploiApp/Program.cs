using Microsoft.EntityFrameworkCore;
using PoleEmploiApp.DataEntities;
using PoleEmploiApp.DataEntities.Repositories.Interfaces;
using PoleEmploiApp.Services;
using PoleEmploiApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IJobOfferService, JobOfferService>();
builder.Services.AddScoped<IPoleEmploiAPIService, PoleEmploiAPIService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// TODO => add connection string on condig file
string sql = "Server=database-1.cx5qrvj1ajmz.ca-central-1.rds.amazonaws.com;Database=PoleEmploi;User Id=admin;password=HelloWork;Trusted_Connection=False;MultipleActiveResultSets=true;Connection Timeout=120;";
builder.Services.AddDbContext<PoleEmploiContext>(options => options.UseSqlServer(sql));

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
