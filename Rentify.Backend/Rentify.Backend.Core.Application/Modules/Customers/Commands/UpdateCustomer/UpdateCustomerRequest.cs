namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;

using Rentify.Backend.Core.Domain.Enums;

public sealed record UpdateCustomerRequest(
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
    string? AddressReference);
