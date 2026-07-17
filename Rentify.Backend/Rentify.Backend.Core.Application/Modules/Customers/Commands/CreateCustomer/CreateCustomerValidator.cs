using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.CustomerType).IsInEnum();
        RuleFor(x => x.IdentificationType).IsInEnum();
        RuleFor(x => x.IdentificationNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.AddressLine).MaximumLength(250);
        RuleFor(x => x.Sector).MaximumLength(100);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.Province).MaximumLength(100);
        RuleFor(x => x.AddressReference).MaximumLength(250);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
