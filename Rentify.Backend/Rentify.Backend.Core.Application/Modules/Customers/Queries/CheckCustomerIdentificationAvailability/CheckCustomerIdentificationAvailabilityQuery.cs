using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.CheckCustomerIdentificationAvailability;

public sealed record CheckCustomerIdentificationAvailabilityQuery(
    Guid TenantId,
    IdentificationType IdentificationType,
    string IdentificationNumber,
    Guid? ExcludeCustomerId) : IRequest<ResultReponse<CustomerAvailabilityResponse>>;
