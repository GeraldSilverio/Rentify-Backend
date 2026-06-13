using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.CreateEmailTemplate
{
    public static class CreateEmailTemplateEndpoint
    {
        public static IEndpointRouteBuilder MapCreateEmailTemplate(this IEndpointRouteBuilder app)
        {
            app.MapPost("/templates",
                async (CreateEmailTemplateCommand command, ISender sender) =>
                {
                    var response = await sender.Send(command);

                    return Results.Created("", response);
                });

            return app;
        }
    }
}
