using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.CheckCustomerEmailAvailability;

public sealed class CheckCustomerEmailAvailabilityValidator
    : AbstractValidator<CheckCustomerEmailAvailabilityQuery>
{
    public CheckCustomerEmailAvailabilityValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
    }
}
