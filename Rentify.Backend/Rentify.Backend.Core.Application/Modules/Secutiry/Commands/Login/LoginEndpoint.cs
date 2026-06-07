using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Core.Application.Modules.Secutiry.Commands.Login
{
    public static class LoginEndpoint
    {
        public static IEndpointRouteBuilder MapLogin(this IEndpointRouteBuilder app)
        {
            app.MapPost("/login",
                async (LoginCommand command, ISender sender) =>
                {
                    var response = await sender.Send(command);

                    return Results.Ok(response);

                }).WithTags("Auth");

            return app;
        }
    }
}
