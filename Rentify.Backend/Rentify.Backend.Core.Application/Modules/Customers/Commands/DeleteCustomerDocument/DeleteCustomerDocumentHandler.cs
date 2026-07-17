using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Services;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomerDocument;

public sealed class DeleteCustomerDocumentHandler : IRequestHandler<DeleteCustomerDocumentCommand>
{
    private readonly ICustomerService _customerService;

    public DeleteCustomerDocumentHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public Task Handle(DeleteCustomerDocumentCommand request, CancellationToken cancellationToken)
    {
        return _customerService.DeleteDocumentAsync(request, cancellationToken);
    }
}
