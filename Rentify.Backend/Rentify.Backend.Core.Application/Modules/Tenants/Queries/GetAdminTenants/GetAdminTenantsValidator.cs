using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenants;

public sealed class GetAdminTenantsValidator : AbstractValidator<GetAdminTenantsQuery>
{
    public GetAdminTenantsValidator()
    {
        RuleFor(x => x.Search).MaximumLength(150);
        RuleFor(x => x.BusinessModel).IsInEnum();
        RuleFor(x => x.SubscriptionStatus).IsInEnum();
        RuleFor(x => x.PlanCode).MaximumLength(50);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
