using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UnverifyCustomer;

public sealed class UnverifyCustomerValidator : AbstractValidator<UnverifyCustomerCommand>
{
    public UnverifyCustomerValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
