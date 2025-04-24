using Films.Context;
using static Films.Services.TmbdService;
using Microsoft.EntityFrameworkCore;
using Films.Models;
using Films.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using DbContext = Films.Context.FilmsDbContext;

var builder = WebApplication.CreateBuilder(args);

// APARTADO DE THE MOVIE DATABASE
builder.Services.AddHttpClient("TMDb", client =>
{
    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
});

builder.Services.AddScoped<TmdbService>();
// FIN DEL APARTADO DE THE MOVIE DATABASE

// Add services to the container.
builder.Services.AddControllersWithViews();

// Sesions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// DbContext settings

builder.Services.AddDbContext<FilmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddAuthentication()
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Autentication/Login";
    });

// Cloudinary Settings
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings")
);
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseSession();
app.UseRouting();          
app.UseAuthentication();   
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
