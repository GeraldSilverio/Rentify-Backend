using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UploadCustomerDocument;

public sealed class UploadCustomerDocumentHandler : IRequestHandler<UploadCustomerDocumentCommand, ResultReponse<Guid>>
{
    private readonly ICustomerService _customerService;

    public UploadCustomerDocumentHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<ResultReponse<Guid>> Handle(UploadCustomerDocumentCommand request, CancellationToken cancellationToken)
    {
        Guid documentId = await _customerService.UploadDocumentAsync(request, cancellationToken);
        return ResultReponse<Guid>.Success(documentId);
    }
}
