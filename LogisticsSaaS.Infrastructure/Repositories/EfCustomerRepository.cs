using LogisticsSaaS.Core.Application.Interfaces;
using LogisticsSaaS.Core.Domain.Entities;
using LogisticsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisticsSaaS.Infrastructure.Repositories;

public class EfCustomerRepository : ICustomerRepository
{
    private readonly LogisticsDbContext _context;

    public EfCustomerRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(string id)
    {
        return await _context.Customers.FindAsync(id);
    }
}
