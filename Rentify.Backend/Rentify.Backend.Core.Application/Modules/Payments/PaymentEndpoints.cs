using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Payments.Commands.RegisterPayment;

namespace Rentify.Backend.Core.Application.Modules.Payments;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapRegisterPaymentEndpoint();
        return app;
    }
}
