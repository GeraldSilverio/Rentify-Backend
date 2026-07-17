using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Customers;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Modules.Customers.Queries.SearchCustomers;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task AddDocumentAsync(CustomerDocument document, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default);
    Task<CustomerDocument?> GetDocumentByIdAsync(Guid tenantId, Guid customerId, Guid documentId, CancellationToken cancellationToken = default);
    Task<bool> IdentificationExistsAsync(Guid tenantId, IdentificationType identificationType, string identificationNumberNormalized, Guid? excludedCustomerId, CancellationToken cancellationToken = default);
    Task<CustomerDetailsResponse?> GetDetailsAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<CustomerResponse>> SearchAsync(SearchCustomersQuery query, CancellationToken cancellationToken = default);
}
