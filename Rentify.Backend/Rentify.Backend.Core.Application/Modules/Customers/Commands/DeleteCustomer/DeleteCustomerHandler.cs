using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, ResultReponse<bool>>
{
    private readonly ICustomerService _customerService;

    public DeleteCustomerHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<ResultReponse<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        await _customerService.DeleteAsync(request, cancellationToken);
        return ResultReponse<bool>.Success(true);
    }
}
