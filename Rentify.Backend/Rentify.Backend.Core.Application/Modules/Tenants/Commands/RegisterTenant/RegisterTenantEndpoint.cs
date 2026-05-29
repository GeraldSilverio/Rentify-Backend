using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public static class RegisterTenantEndpoint
{
    public static IEndpointRouteBuilder MapRegisterTenant(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/v1/tenant/register-tenant",
            async (
                RegisterTenantCommand command,
                ISender sender) =>
            {
                var response =
                    await sender.Send(command);

                return Results.Created("", response);
            });
        return app;
    }
}