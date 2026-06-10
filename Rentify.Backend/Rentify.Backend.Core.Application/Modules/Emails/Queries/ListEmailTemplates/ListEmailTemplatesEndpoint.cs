using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Emails.Queries.ListEmailTemplates
{
    public static class ListEmailTemplatesEndpoint
    {
        public static IEndpointRouteBuilder MapListEmailTemplates(this IEndpointRouteBuilder app)
        {
            app.MapGet("/templates",
                async (Guid? tenantId, ISender sender) =>
                {
                    var response = await sender.Send(new ListEmailTemplatesQuery(tenantId));

                    return Results.Ok(response);
                });

            return app;
        }
    }
}
