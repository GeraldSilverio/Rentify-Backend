using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser
{
    public static class CreateUserEndPoint
    {
        public static IEndpointRouteBuilder MapCreateUser(this IEndpointRouteBuilder app)
        {
            app.MapPost("/create-user",
                async (CreateUserCommand command, ISender sender) =>
                {
                    var response = await sender.Send(command);

                    return Results.Created("", response);

                }).WithTags("User");

            return app;
        }

    }
}
