using WEATHERAPP.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================
// 1?? ≈÷«›… «·Œœ„« 
// ============================

// MVC
builder.Services.AddControllersWithViews();

// Session
builder.Services.AddDistributedMemoryCache(); // ÷—Ê—Ì · Œ“Ì‰ «·Ã·”… »«·–«ﬂ—…
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // „œ… »ﬁ«¡ «·Ã·”…
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// DatabaseService »«” Œœ«„ Factory Lambda (Õ· „‘ﬂ·… AddSingleton)
builder.Services.AddSingleton<DatabaseService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new DatabaseService(config);
});

// ? ≈÷«›… HttpClientFactory · Ã‰» Œÿ√ Unable to resolve service for type 'IHttpClientFactory'
builder.Services.AddHttpClient();

var app = builder.Build();

// ============================
// 2?? ≈⁄œ«œ «·‹ HTTP request pipeline
// ============================

// „·›«  À«» … „À· CSS Ê JS
app.UseStaticFiles();

app.UseRouting();

// Session ·«“„ ÌﬂÊ‰ ﬁ»· Authorization
app.UseSession();

// Authorization
app.UseAuthorization();

// Route «·«› —«÷Ì
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();
