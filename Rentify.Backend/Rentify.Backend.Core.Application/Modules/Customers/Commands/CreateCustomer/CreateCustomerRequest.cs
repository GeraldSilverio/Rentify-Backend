namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string LicenseNumber,
    DateOnly LicenseExpirationDate,
    string CreatedBy);
