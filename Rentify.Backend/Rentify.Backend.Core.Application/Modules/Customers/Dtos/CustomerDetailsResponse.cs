namespace Rentify.Backend.Core.Application.Modules.Customers.Dtos;

using Rentify.Backend.Core.Domain.Enums;

public sealed record CustomerDetailsResponse(
    Guid Id,
    Guid TenantId,
    CustomerType CustomerType,
    IdentificationType IdentificationType,
    string IdentificationNumber,
    DateOnly? BirthDate,
    bool IsVerified,
    DateTime? VerifiedAt,
    string? VerifiedBy,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? AddressLine,
    string? Sector,
    string? City,
    string? Province,
    string? AddressReference,
    bool IsActive,
    DateTime CreatedDate,
    DateTime ModifiedDate,
    IReadOnlyList<CustomerDocumentResponse> Documents);
