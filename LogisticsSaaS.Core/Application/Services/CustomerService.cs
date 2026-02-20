using LogisticsSaaS.Core.Application.Interfaces;
using LogisticsSaaS.Core.Domain.Entities;

namespace LogisticsSaaS.Core.Application.Services;

public class CustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Customer>> GetCustomersAsync() => _repository.GetAllAsync();
}
