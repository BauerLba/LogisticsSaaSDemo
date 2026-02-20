using LogisticsSaaS.Core.Application.Interfaces;
using LogisticsSaaS.Core.Domain.Entities;

namespace LogisticsSaaS.Infrastructure.Repositories;

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers = new()
    {
        new Customer { Id = "CUST-001", Name = "Tech Nova Solutions", Email = "ops@technova.com", Location = "San Francisco, US", ActiveShipments = 12, TotalSpent = 45200.50m, Icon = "fa-microchip", Color = "#6366f1" },
        new Customer { Id = "CUST-002", Name = "Global Trade Co", Email = "logistics@globaltrade.net", Location = "London, UK", ActiveShipments = 8, TotalSpent = 28400.00m, Icon = "fa-globe", Color = "#10b981" },
        new Customer { Id = "CUST-003", Name = "Eco Logistics", Email = "green@ecolog.de", Location = "Berlin, DE", ActiveShipments = 4, TotalSpent = 12150.75m, Icon = "fa-leaf", Color = "#f59e0b" },
        new Customer { Id = "CUST-004", Name = "Apex Systems", Email = "shipping@apex.jp", Location = "Tokyo, JP", ActiveShipments = 25, TotalSpent = 89000.00m, Icon = "fa-server", Color = "#ec4899" }
    };

    public Task<IEnumerable<Customer>> GetAllAsync() => Task.FromResult<IEnumerable<Customer>>(_customers);

    public Task<Customer?> GetByIdAsync(string id) => Task.FromResult(_customers.FirstOrDefault(c => c.Id == id));
}
