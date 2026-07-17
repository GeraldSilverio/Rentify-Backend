using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.VerifyCustomer;

public sealed class VerifyCustomerHandler : IRequestHandler<VerifyCustomerCommand, VerifyCustomerResponse>
{
    private readonly ICustomerService _customerService;

    public VerifyCustomerHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public Task<VerifyCustomerResponse> Handle(VerifyCustomerCommand request, CancellationToken cancellationToken)
    {
        return _customerService.VerifyAsync(request, cancellationToken);
    }
}
