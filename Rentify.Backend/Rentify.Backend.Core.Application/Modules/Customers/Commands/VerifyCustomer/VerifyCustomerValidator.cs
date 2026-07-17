using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.VerifyCustomer;

public sealed class VerifyCustomerValidator : AbstractValidator<VerifyCustomerCommand>
{
    public VerifyCustomerValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.VerifiedBy).NotEmpty();
    }
}
