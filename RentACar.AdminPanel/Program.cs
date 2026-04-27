using RentACar.AdminPanel.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC Controller ve View desteği
builder.Services.AddControllersWithViews();

// 2. HttpContextAccessor — Cookie'den token okumak için gerekli
builder.Services.AddHttpContextAccessor();

// 3. Session ayarları
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. HttpClient ve BaseApiService kaydı
//    BaseAddress artık constructor'da appsettings'ten okunuyor
builder.Services.AddHttpClient<BaseApiService>();

// 5. Cookie tabanlı kimlik doğrulama
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "RentACarAdminCookie";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection(); // HTTPS yalnızca production'da zorunlu
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// Sıralama önemli: Authentication önce, Authorization sonra
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();