using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Emails.Queries.GetEmailTemplate
{
    public static class GetEmailTemplateEndpoint
    {
        public static IEndpointRouteBuilder MapGetEmailTemplate(this IEndpointRouteBuilder app)
        {
            app.MapGet("/templates/{id:guid}",
                async (Guid id, ISender sender) =>
                {
                    var response = await sender.Send(new GetEmailTemplateQuery(id));

                    return Results.Ok(response);
                });

            return app;
        }
    }
}
