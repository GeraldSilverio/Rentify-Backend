using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.LicenseNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LicenseExpirationDate).GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
