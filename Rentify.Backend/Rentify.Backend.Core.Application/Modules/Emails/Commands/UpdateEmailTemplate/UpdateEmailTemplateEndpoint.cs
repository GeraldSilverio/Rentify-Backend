using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.UpdateEmailTemplate
{
    public static class UpdateEmailTemplateEndpoint
    {
        public static IEndpointRouteBuilder MapUpdateEmailTemplate(this IEndpointRouteBuilder app)
        {
            app.MapPut("/templates/{id:guid}",
                async (Guid id, UpdateEmailTemplateRequest request, ISender sender) =>
                {
                    var command = new UpdateEmailTemplateCommand(
                        id,
                        request.Name,
                        request.Subject,
                        request.HtmlBody,
                        request.TextBody,
                        request.ModifiedBy);

                    var response = await sender.Send(command);

                    return Results.Ok(response);
                });

            return app;
        }
    }
}
