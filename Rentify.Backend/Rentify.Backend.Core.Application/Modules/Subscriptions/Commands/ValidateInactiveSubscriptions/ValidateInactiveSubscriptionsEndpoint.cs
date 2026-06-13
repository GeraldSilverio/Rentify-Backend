using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Commands.ValidateInactiveSubscriptions
{
    public static class ValidateInactiveSubscriptionsEndpoint
    {
        public static IEndpointRouteBuilder MapValidateInactiveSubscriptions(this IEndpointRouteBuilder app)
        {
            app.MapPost("/validate-inactive",
                async (ISender sender) =>
                {
                    var response = await sender.Send(new ValidateInactiveSubscriptionsCommand());

                    return Results.Ok(response);
                });

            return app;
        }
    }
}
