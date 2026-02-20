using LogisticsSaaS.Core.Domain.Entities;

namespace LogisticsSaaS.Core.Application.Interfaces;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(string id);
}
