
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();
