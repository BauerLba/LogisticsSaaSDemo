using LogisticsSaaS.Web.Components;
using LogisticsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LogisticsSaaS.Core.Application.Interfaces;
using LogisticsSaaS.Infrastructure.Repositories;
using LogisticsSaaS.Core.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL"); // Render often uses DATABASE_URL

if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<LogisticsDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // Fallback for local development if no DB is configured
    builder.Services.AddDbContext<LogisticsDbContext>(options =>
        options.UseInMemoryDatabase("LogisticsSaaS"));
}

// Register Clean Architecture Layers
builder.Services.AddScoped<IShipmentRepository, EfShipmentRepository>();
builder.Services.AddScoped<ICustomerRepository, EfCustomerRepository>();
builder.Services.AddScoped<ShipmentService>();
builder.Services.AddScoped<CustomerService>();

var app = builder.Build();

// Automatically apply migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LogisticsDbContext>();
    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }

    // Seed data if empty
    SeedData(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// Simple Seeding logic for the demo
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

