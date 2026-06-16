namespace Rentify.Backend.Core.Application.Modules.Customers.Dtos;

public sealed record CustomerResponse(
    Guid Id,
    Guid TenantId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string LicenseNumber,
    DateOnly LicenseExpirationDate);
