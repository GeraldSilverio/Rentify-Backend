using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail
{
    public static class SendTemplateEmailEndpoint
    {
        public static IEndpointRouteBuilder MapSendTemplateEmail(this IEndpointRouteBuilder app)
        {
            app.MapPost("/send-template",
                async (SendTemplateEmailCommand command, ISender sender) =>
                {
                    var response = await sender.Send(command);

                    return Results.Ok(response);
                });

            return app;
        }
    }
}
