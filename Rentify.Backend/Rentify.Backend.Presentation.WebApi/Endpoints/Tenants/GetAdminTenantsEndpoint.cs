using MediatR;
using Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenants;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class GetAdminTenantsEndpoint
{
    public static IEndpointRouteBuilder MapGetAdminTenantsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(string.Empty, async (
            ISender sender,
            CancellationToken cancellationToken,
            string? search = null,
            BusinessModel? businessModel = null,
            bool? isActive = null,
            SubscriptionStatus? subscriptionStatus = null,
            string? planCode = null,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var response = await sender.Send(
                new GetAdminTenantsQuery(
                    search,
                    businessModel,
                    isActive,
                    subscriptionStatus,
                    planCode,
                    pageNumber,
                    pageSize),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetAdminTenants")
        .WithSummary("Lists tenants for Super Admin with filters and pagination.");

        return app;
    }
}
