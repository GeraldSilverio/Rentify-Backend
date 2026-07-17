using MediatR;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomerDocument;

public sealed record DeleteCustomerDocumentCommand(
    Guid TenantId,
    Guid CustomerId,
    Guid DocumentId,
    string ModifiedBy) : IRequest;
