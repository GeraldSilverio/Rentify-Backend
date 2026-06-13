using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Emails.Queries.ListTenantEmailConfigurations
{
    public static class ListTenantEmailConfigurationsEndpoint
    {
        public static IEndpointRouteBuilder MapListTenantEmailConfigurations(this IEndpointRouteBuilder app)
        {
            app.MapGet("/tenant-configurations/{tenantId:guid}",
                async (Guid tenantId, ISender sender) =>
                {
                    var response = await sender.Send(new ListTenantEmailConfigurationsQuery(tenantId));

                    return Results.Ok(response);
                });

            return app;
        }
    }
}
