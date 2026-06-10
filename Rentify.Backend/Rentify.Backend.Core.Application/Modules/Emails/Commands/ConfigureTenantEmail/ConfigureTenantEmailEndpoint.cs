using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.ConfigureTenantEmail
{
    public static class ConfigureTenantEmailEndpoint
    {
        public static IEndpointRouteBuilder MapConfigureTenantEmail(this IEndpointRouteBuilder app)
        {
            app.MapPost("/tenant-configurations",
                async (ConfigureTenantEmailCommand command, ISender sender) =>
                {
                    var response = await sender.Send(command);

                    return Results.Created("", response);
                });

            return app;
        }
    }
}
