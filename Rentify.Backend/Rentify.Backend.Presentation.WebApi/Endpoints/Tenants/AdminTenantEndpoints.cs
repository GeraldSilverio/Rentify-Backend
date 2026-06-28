using Rentify.Backend.Core.Application.Modules.Shared.Constants;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class AdminTenantEndpoints
{
    public static IEndpointRouteBuilder MapAdminTenantEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/v1/admin/tenants")
            .RequireAuthorization(policy => policy.RequireRole(ApplicationRoles.SuperAdmin))
            .WithTags("Admin Tenants");

        group.MapGetAdminTenantsEndpoint();
        group.MapGetAdminTenantDetailEndpoint();
        group.MapGetAdminTenantSubscriptionEndpoint();
        group.MapUpdateAdminTenantEndpoint();
        group.MapDeactivateAdminTenantEndpoint();
        group.MapActivateAdminTenantEndpoint();
        group.MapExtendTenantTrialEndpoint();

        return app;
    }
}
