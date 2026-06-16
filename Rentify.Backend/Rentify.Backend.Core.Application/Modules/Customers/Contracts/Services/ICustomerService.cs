using Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UploadCustomerDocument;

namespace Rentify.Backend.Core.Application.Modules.Customers.Contracts.Services;

public interface ICustomerService
{
    Task<Guid> CreateAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeleteCustomerCommand command, CancellationToken cancellationToken = default);
    Task<Guid> UploadDocumentAsync(UploadCustomerDocumentCommand command, CancellationToken cancellationToken = default);
}
