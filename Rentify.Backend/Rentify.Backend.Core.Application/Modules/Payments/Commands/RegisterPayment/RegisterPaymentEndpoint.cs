using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Payments.Commands.RegisterPayment;

public static class RegisterPaymentEndpoint
{
    public static IEndpointRouteBuilder MapRegisterPaymentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/tenants/{tenantId:guid}/payments", async (
            Guid tenantId,
            RegisterPaymentRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new RegisterPaymentCommand(
                tenantId,
                request.ReservationId,
                request.Amount,
                request.Method,
                request.Reference,
                request.CreatedBy), cancellationToken);

            return Results.Created($"/api/v1/tenants/{tenantId}/payments/{response.Value?.PaymentId}", response);
        })
        .WithTags("Payments");

        return app;
    }
}
