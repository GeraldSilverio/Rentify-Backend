using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> SearchAsync(Guid tenantId, string? searchTerm, CancellationToken cancellationToken = default);
    Task<bool> LicenseNumberExistsAsync(Guid tenantId, string licenseNumber, Guid? excludedCustomerId = null, CancellationToken cancellationToken = default);
}
