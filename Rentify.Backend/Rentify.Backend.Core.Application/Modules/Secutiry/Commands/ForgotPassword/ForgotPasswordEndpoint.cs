using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ForgotPassword
{
    public static class ForgotPasswordEndpoint
    {
        public static IEndpointRouteBuilder MapForgotPassword(this IEndpointRouteBuilder app)
        {
            app.MapPost("/forgot-password",
                async (ForgotPasswordCommand command, ISender sender) =>
                {
                    var response = await sender.Send(command);

                    return Results.Ok(response);
                }).WithTags("Auth");

            return app;
        }
    }
}
