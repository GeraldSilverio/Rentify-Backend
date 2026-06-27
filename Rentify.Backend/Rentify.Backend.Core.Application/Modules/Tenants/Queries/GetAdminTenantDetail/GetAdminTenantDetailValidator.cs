using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenantDetail;

public sealed class GetAdminTenantDetailValidator : AbstractValidator<GetAdminTenantDetailQuery>
{
    public GetAdminTenantDetailValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
    }
}
