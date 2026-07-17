using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomerDocument;

public sealed class DeleteCustomerDocumentValidator : AbstractValidator<DeleteCustomerDocumentCommand>
{
    public DeleteCustomerDocumentValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.DocumentId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
