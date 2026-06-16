using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
