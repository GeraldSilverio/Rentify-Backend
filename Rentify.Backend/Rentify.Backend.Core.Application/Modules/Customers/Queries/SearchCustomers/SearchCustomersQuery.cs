using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.SearchCustomers;

public sealed record SearchCustomersQuery(
    Guid TenantId,
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null) : IRequest<ResultReponse<PaginatedResponse<CustomerResponse>>>;
