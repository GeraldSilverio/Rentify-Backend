using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(
    Guid TenantId,
    Guid CustomerId) : IRequest<ResultReponse<CustomerDetailsResponse>>;
