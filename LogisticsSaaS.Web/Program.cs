using LogisticsSaaS.Web.Components;
using LogisticsSaaS.Web.Hubs;
using LogisticsSaaS.Infrastructure.Data;
using LogisticsSaaS.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LogisticsSaaS.Core.Application.Interfaces;
using LogisticsSaaS.Infrastructure.Repositories;
using LogisticsSaaS.Core.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<LogisticsDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    builder.Services.AddDbContext<LogisticsDbContext>(options =>
        options.UseInMemoryDatabase("LogisticsSaaS"));
}

// ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<LogisticsDbContext>()
.AddDefaultTokenProviders();

// Cookie auth
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/login";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization();

// Register Clean Architecture Layers
builder.Services.AddScoped<IShipmentRepository, EfShipmentRepository>();
builder.Services.AddScoped<ICustomerRepository, EfCustomerRepository>();
builder.Services.AddSingleton<AuditService>();
builder.Services.AddScoped<ShipmentService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AppSettingsService>();

var app = builder.Build();

// Automatically apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LogisticsDbContext>();
    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }
    SeedData(dbContext);
    await SeedDemoUserAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Login endpoint (Blazor Server cannot set cookies over SignalR)
app.MapPost("/do-login", async (HttpContext ctx, SignInManager<ApplicationUser> signInManager) =>
{
    var email = ctx.Request.Form["email"].ToString();
    var password = ctx.Request.Form["password"].ToString();
    var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: false);
    return result.Succeeded ? Results.Redirect("/") : Results.Redirect("/login?error=1");
}).DisableAntiforgery();

// Register endpoint
app.MapPost("/do-register", async (HttpContext ctx, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) =>
{
    var displayName = ctx.Request.Form["displayName"].ToString();
    var email = ctx.Request.Form["email"].ToString();
    var password = ctx.Request.Form["password"].ToString();
    var confirmPassword = ctx.Request.Form["confirmPassword"].ToString();

    if (password != confirmPassword)
        return Results.Redirect("/register?error=password");

    var user = new ApplicationUser
    {
        UserName = email,
        Email = email,
        DisplayName = displayName,
        CreatedAt = DateTime.UtcNow
    };

    var result = await userManager.CreateAsync(user, password);
    if (result.Succeeded)
    {
        await signInManager.SignInAsync(user, isPersistent: true);
        return Results.Redirect("/");
    }

    var errorMsg = Uri.EscapeDataString(string.Join(" ", result.Errors.Select(e => e.Description)));
    return Results.Redirect($"/register?error={errorMsg}");
}).DisableAntiforgery();

// Logout endpoint
app.MapGet("/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/login");
});

app.MapHub<ShipmentHub>("/hub/shipments");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

async Task SeedDemoUserAsync(IServiceProvider services)
{
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    const string email = "demo@logiflow.com";
    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = "Demo User",
            CreatedAt = DateTime.UtcNow
        };
        await userManager.CreateAsync(user, "Demo123!");
    }
}

void SeedData(LogisticsDbContext context)
{
    if (!context.Customers.Any())
    {
        context.Customers.AddRange(
            new LogisticsSaaS.Core.Domain.Entities.Customer { Id = "CUST001", Name = "Global Trade Corp", Email = "contact@globaltrade.com", Location = "New York, USA", ActiveShipments = 12, TotalSpent = 45000.50m },
            new LogisticsSaaS.Core.Domain.Entities.Customer { Id = "CUST002", Name = "Euro Logistics GmbH", Email = "info@eurologistics.de", Location = "Berlin, Germany", ActiveShipments = 8, TotalSpent = 28300.75m }
        );
        context.SaveChanges();
    }
}
