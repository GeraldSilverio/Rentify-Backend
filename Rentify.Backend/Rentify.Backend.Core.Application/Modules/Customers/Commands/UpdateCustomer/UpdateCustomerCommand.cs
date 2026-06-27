using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid TenantId,
    Guid CustomerId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string LicenseNumber,
    DateOnly LicenseExpirationDate,
    string ModifiedBy) : IRequest<ResultReponse<Guid>>;
