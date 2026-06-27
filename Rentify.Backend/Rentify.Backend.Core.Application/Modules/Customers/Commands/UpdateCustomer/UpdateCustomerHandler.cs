using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, ResultReponse<Guid>>
{
    private readonly ICustomerService _customerService;

    public UpdateCustomerHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<ResultReponse<Guid>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        Guid customerId = await _customerService.UpdateAsync(request, cancellationToken);
        return ResultReponse<Guid>.Success(customerId);
    }
}
