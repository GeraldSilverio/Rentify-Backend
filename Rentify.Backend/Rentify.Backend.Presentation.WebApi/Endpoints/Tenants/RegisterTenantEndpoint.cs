using MediatR;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;

public static class RegisterTenantEndpoint
{
    public static IEndpointRouteBuilder MapRegisterTenant(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/v1/tenant",
            async (
                RegisterTenantCommand command,
                ISender sender) =>
            {
                var response =
                    await sender.Send(command);

                return Results.Created("", response);
            }).WithTags("Tenant");
        return app;
    }
}
