using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomer;

public sealed record DeleteCustomerCommand(
    Guid TenantId,
    Guid CustomerId,
    string ModifiedBy) : IRequest<ResultReponse<bool>>;
