using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetSubscriptionPlans
{
    public static class GetSubscriptionPlanQueryEndpoint
    {
        public static IEndpointRouteBuilder MapGetSubscriptionPlan(this IEndpointRouteBuilder app)
        {
            app.MapGet("/subscription-plans",
                async (ISender sender) =>
                {
                    var response = await sender.Send(new GetSubscriptionPlanQuery());
                    return Results.Ok(response);
                });
            return app;
        }
    }
}
