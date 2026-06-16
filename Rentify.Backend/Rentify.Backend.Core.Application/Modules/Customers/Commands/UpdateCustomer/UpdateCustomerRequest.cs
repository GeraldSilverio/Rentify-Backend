namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string LicenseNumber,
    DateOnly LicenseExpirationDate,
    string ModifiedBy);
