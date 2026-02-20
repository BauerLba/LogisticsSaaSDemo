using LogisticsSaaS.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register Clean Architecture Layers
builder.Services.AddSingleton<LogisticsSaaS.Core.Application.Interfaces.IShipmentRepository, LogisticsSaaS.Infrastructure.Repositories.InMemoryShipmentRepository>();
builder.Services.AddSingleton<LogisticsSaaS.Core.Application.Interfaces.ICustomerRepository, LogisticsSaaS.Infrastructure.Repositories.InMemoryCustomerRepository>();
builder.Services.AddScoped<LogisticsSaaS.Core.Application.Services.ShipmentService>();
builder.Services.AddScoped<LogisticsSaaS.Core.Application.Services.CustomerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
