using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Dashboard.Queries.GetDashboardMetrics;

public static class GetDashboardMetricsEndpoint
{
    public static IEndpointRouteBuilder MapGetDashboardMetricsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/tenants/{tenantId:guid}/dashboard/metrics", async (
            Guid tenantId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetDashboardMetricsQuery(tenantId), cancellationToken);
            return Results.Ok(response);
        })
        .WithTags("Dashboard");

        return app;
    }
}
