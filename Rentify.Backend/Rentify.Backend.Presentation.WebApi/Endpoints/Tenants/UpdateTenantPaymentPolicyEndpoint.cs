using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantPaymentPolicy;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class UpdateTenantPaymentPolicyEndpoint
{
    public static IEndpointRouteBuilder MapUpdateTenantPaymentPolicyEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/payment-policy", async (
            UpdateTenantPaymentPolicyRequest request,
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new UpdateTenantPaymentPolicyCommand(
                    currentTenantService.GetTenantId(),
                    request.Name,
                    request.PaymentFrequency,
                    request.CutoffDayOfWeek,
                    request.GraceDays,
                    request.ReminderStartDayOfWeek,
                    request.LateFeeEnabled,
                    currentUserService.ModifiedBy),
                cancellationToken);

            return Results.Ok(response);
        })
        .WithName("UpdateTenantPaymentPolicy")
        .WithSummary("Updates the default payment policy for the authenticated tenant.");

        return app;
    }
}
