using static Films.Services.TmbdService;

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

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
