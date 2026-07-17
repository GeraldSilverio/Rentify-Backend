using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UnverifyCustomer;

public sealed class UnverifyCustomerHandler : IRequestHandler<UnverifyCustomerCommand, UnverifyCustomerResponse>
{
    private readonly ICustomerService _customerService;

    public UnverifyCustomerHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public Task<UnverifyCustomerResponse> Handle(UnverifyCustomerCommand request, CancellationToken cancellationToken)
    {
        return _customerService.UnverifyAsync(request, cancellationToken);
    }
}
