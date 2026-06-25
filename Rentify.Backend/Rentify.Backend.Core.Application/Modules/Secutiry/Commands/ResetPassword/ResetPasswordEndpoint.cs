using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.ResetPassword
{
    public static class ResetPasswordEndpoint
    {
        public static IEndpointRouteBuilder MapResetPassword(this IEndpointRouteBuilder app)
        {
            app.MapPost("/reset-password",
                async (ResetPasswordCommand command, ISender sender) =>
                {
                    var response = await sender.Send(command);

                    return Results.Ok(response);
                }).WithTags("Authentication");

            return app;
        }
    }
}
