using Microsoft.EntityFrameworkCore;
using StudentPortalDBFirst.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using Microsoft.AspNetCore.Identity;
using NuGet.Configuration;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container
builder.Services.AddControllersWithViews();

// 2. Register EF DB Contexts
builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StudentDbContext")));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext")));

// 3. Register Session Support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // Only use Always in production with HTTPS
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.None
        : CookieSecurePolicy.Always;
});

// 4. Authentication - FIXED with proper settings
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/UsersMasters/Login";
    options.LogoutPath = "/UsersMasters/Logout";
    options.AccessDeniedPath = "/UsersMasters/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.Cookie.Name = "StudentPortalAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // Only use Always in production with HTTPS
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.None
        : CookieSecurePolicy.Always;

    // This event handler can help with debugging auth issues
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            // For API requests that should return 401 instead of redirecting
            if (context.Request.Path.StartsWithSegments("/api") &&
                context.Response.StatusCode == 200)
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    };
});

// 5. Authorization
builder.Services.AddAuthorization();     // This enables you to use [Authorize] on controllers to block access if not logged in.




// ✅ Password Hasher registration - MUST be before builder.Build()
builder.Services.AddScoped<IPasswordHasher<UsersMaster>, PasswordHasher<UsersMaster>>();




// 6. Other services
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.None
        : CookieSecurePolicy.Always;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// 7. Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// 8. Middleware pipeline in correct order
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Session must come before auth
app.UseSession();

// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();         // Use[Authorize] attributes to protect pages
                                // Only logged-in users can see/manage data and  Prevents access to features like Edit/Delete unless logged in
                                //It protects routes, pages, and actions from unauthenticated users.
                                //



// 9. Endpoints
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");




app.Run();