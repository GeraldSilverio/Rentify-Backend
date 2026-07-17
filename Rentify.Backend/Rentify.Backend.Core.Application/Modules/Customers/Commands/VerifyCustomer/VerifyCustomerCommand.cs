using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.VerifyCustomer;

public sealed record VerifyCustomerCommand(
    Guid TenantId,
    Guid CustomerId,
    string VerifiedBy) : IRequest<VerifyCustomerResponse>;
