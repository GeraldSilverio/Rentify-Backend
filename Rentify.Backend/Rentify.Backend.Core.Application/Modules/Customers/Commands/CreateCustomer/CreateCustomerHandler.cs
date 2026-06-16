using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, ResultReponse<Guid>>
{
    private readonly ICustomerService _customerService;

    public CreateCustomerHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<ResultReponse<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        Guid customerId = await _customerService.CreateAsync(request, cancellationToken);
        return ResultReponse<Guid>.Success(customerId);
    }
}
