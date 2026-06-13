using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RefreshToken
{
    public static class RefreshTokenEndpoint
    {
        public static IEndpointRouteBuilder MapRefreshToken(this IEndpointRouteBuilder app)
        {
            app.MapPost("/refresh-token",
                async (RefreshTokenCommand command, ISender sender) =>
                {
                    var response = await sender.Send(command);

                    return Results.Ok(response);
                }).WithTags("Auth");

            return app;
        }
    }
}
