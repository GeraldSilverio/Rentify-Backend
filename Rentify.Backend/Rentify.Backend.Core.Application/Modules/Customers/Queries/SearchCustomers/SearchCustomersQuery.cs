using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.SearchCustomers;

public sealed record SearchCustomersQuery(
    Guid TenantId,
    string? SearchTerm) : IRequest<ResultReponse<IReadOnlyList<CustomerResponse>>>;
