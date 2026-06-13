using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.RevokeRefreshToken
{
    public static class RevokeRefreshTokenEndpoint
    {
        public static IEndpointRouteBuilder MapRevokeRefreshToken(this IEndpointRouteBuilder app)
        {
            app.MapPost("/revoke-refresh-token",
                async (RevokeRefreshTokenCommand command, ISender sender) =>
                {
                    var response = await sender.Send(command);

                    return Results.Ok(response);
                })
                .RequireAuthorization()
                .WithTags("Auth");

            return app;
        }
    }
}
