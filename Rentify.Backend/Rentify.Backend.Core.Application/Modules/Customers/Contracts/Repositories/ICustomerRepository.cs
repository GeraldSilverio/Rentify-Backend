using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Customers;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Modules.Customers.Queries.SearchCustomers;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default);
    Task<CustomerDetailsResponse?> GetDetailsAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<CustomerResponse>> SearchAsync(SearchCustomersQuery query, CancellationToken cancellationToken = default);
}
