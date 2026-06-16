using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Dashboard.Queries.GetDashboardMetrics;

namespace Rentify.Backend.Core.Application.Modules.Dashboard;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetDashboardMetricsEndpoint();
        return app;
    }
}
