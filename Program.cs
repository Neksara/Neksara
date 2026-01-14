using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Neksara.Data;
using Neksara.Services;
using Neksara.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ===== CONNECTION STRING =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ===== DATABASE CONTEXT =====
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// ===== AUTHENTICATION =====
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// ===== CONTROLLERS & VIEWS =====
builder.Services.AddControllersWithViews();

// ===== MEMORY CACHE =====
builder.Services.AddMemoryCache();

// ===== CACHE & SESSION =====
builder.Services.AddDistributedMemoryCache(); // wajib kalau mau pakai session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // session berlaku 30 menit
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ===== REGISTER SERVICES =====
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IAdminEcatalogService, AdminEcatalogService>();
builder.Services.AddScoped<ILearningService, LearningService>();
builder.Services.AddScoped<IAdminTestimoniService, AdminTestimoniService>();
builder.Services.AddScoped<ITestimoniService, TestimoniService>();

var app = builder.Build();

// ===== MIDDLEWARE =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// **PASTIKAN SESSION DIPANGGIL SEBELUM AUTHENTICATION**
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ===== ROUTING =====
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ===== MIGRATE & SEED DATABASE =====
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        // ✅ SEED ADMIN (opsional)
        Neksara.Data.Seeders.UserSeeder.Seed(context);

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error saat migrasi atau seed DB.");
    }
}

app.Run();
