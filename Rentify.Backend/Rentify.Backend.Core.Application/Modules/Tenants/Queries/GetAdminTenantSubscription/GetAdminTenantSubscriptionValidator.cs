using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenantSubscription;

public sealed class GetAdminTenantSubscriptionValidator : AbstractValidator<GetAdminTenantSubscriptionQuery>
{
    public GetAdminTenantSubscriptionValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
    }
}
