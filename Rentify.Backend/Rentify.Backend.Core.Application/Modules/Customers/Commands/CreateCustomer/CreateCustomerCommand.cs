using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    Guid TenantId,
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
    string CreatedBy) : IRequest<ResultReponse<Guid>>;
