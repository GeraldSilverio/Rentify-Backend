using Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UploadCustomerDocument;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.VerifyCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UnverifyCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomerDocument;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Customers.Contracts.Services;

public interface ICustomerService
{
    Task<Guid> CreateAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeleteCustomerCommand command, CancellationToken cancellationToken = default);
    Task<Guid> UploadDocumentAsync(UploadCustomerDocumentCommand command, CancellationToken cancellationToken = default);
    Task<VerifyCustomerResponse> VerifyAsync(VerifyCustomerCommand command, CancellationToken cancellationToken = default);
    Task<UnverifyCustomerResponse> UnverifyAsync(UnverifyCustomerCommand command, CancellationToken cancellationToken = default);
    Task DeleteDocumentAsync(DeleteCustomerDocumentCommand command, CancellationToken cancellationToken = default);
}
