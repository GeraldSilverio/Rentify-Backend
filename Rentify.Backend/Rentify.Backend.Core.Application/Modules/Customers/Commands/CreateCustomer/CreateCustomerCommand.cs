using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    Guid TenantId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string LicenseNumber,
    DateOnly LicenseExpirationDate,
    string CreatedBy) : IRequest<ResultReponse<Guid>>;
