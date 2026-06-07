using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser;

namespace Rentify.Backend.Presentation.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(
        this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api/v1/users")
            .WithTags("Users");
            //.RequireAuthorization();

        users.MapCreateUser();

        // users.MapGetUserById();
        // users.MapUpdateUser();
        // users.MapDeleteUser();

        return app;
    }
}