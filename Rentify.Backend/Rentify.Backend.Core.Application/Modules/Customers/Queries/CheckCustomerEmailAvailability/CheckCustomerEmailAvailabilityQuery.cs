using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.CheckCustomerEmailAvailability;

public sealed record CheckCustomerEmailAvailabilityQuery(
    Guid TenantId,
    string Email,
    Guid? ExcludeCustomerId) : IRequest<ResultReponse<CustomerAvailabilityResponse>>;
