using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UnverifyCustomer;

public sealed record UnverifyCustomerCommand(
    Guid TenantId,
    Guid CustomerId,
    string ModifiedBy) : IRequest<UnverifyCustomerResponse>;
