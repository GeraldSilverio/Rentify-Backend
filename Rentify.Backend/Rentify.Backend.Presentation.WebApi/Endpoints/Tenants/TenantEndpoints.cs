using Rentify.Backend.Core.Application.Modules.Shared.Constants;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class TenantEndpoints
{
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/v1/tenant")
            .RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.Owner, ApplicationRoles.Admin))
            .WithTags("Tenant");

        group.MapGetTenantProfileEndpoint();
        group.MapGetTenantSettingsEndpoint();
        group.MapGetTenantPaymentPolicyEndpoint();
        group.MapGetTenantSubscriptionEndpoint();
        group.MapGetTenantUsageEndpoint();
        group.MapUpdateTenantProfileEndpoint();
        group.MapUpdateTenantSettingsEndpoint();
        group.MapUpdateTenantPaymentPolicyEndpoint();

        return app;
    }
}
