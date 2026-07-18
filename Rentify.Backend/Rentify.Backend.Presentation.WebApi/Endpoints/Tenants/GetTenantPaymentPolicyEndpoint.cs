using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantPaymentPolicy;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class GetTenantPaymentPolicyEndpoint
{
    public static IEndpointRouteBuilder MapGetTenantPaymentPolicyEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/payment-policy", async (
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new GetTenantPaymentPolicyQuery(currentRequestContext.TenantId),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetTenantPaymentPolicy")
        .WithSummary("Gets the default payment policy for the authenticated tenant.");

        return app;
    }
}
