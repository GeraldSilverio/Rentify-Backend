using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid TenantId,
    Guid CustomerId,
    CustomerType CustomerType,
    IdentificationType IdentificationType,
    string IdentificationNumber,
    DateOnly? BirthDate,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? AddressLine,
    string? Sector,
    string? City,
    string? Province,
    string? AddressReference,
    string ModifiedBy) : IRequest<ResultReponse<Guid>>;
