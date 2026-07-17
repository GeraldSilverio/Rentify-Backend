using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.CheckCustomerIdentificationAvailability;

public sealed class CheckCustomerIdentificationAvailabilityValidator
    : AbstractValidator<CheckCustomerIdentificationAvailabilityQuery>
{
    public CheckCustomerIdentificationAvailabilityValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.IdentificationType).IsInEnum();
        RuleFor(x => x.IdentificationNumber).NotEmpty().MaximumLength(30);
    }
}
