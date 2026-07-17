namespace Rentify.Backend.Core.Application.Modules.Customers.Dtos;

using Rentify.Backend.Core.Domain.Enums;

public sealed record CustomerResponse(
    Guid Id,
    Guid TenantId,
    CustomerType CustomerType,
    IdentificationType IdentificationType,
    string IdentificationNumber,
    DateOnly? BirthDate,
    bool IsVerified,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? AddressLine,
    string? Sector,
    string? City,
    string? Province,
    string? AddressReference);
