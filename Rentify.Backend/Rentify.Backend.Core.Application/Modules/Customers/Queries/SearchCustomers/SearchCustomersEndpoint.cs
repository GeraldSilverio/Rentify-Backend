using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Customers.Queries.SearchCustomers;

public static class SearchCustomersEndpoint
{
    public static IEndpointRouteBuilder MapSearchCustomersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/tenants/{tenantId:guid}/customers", async (
            Guid tenantId,
            string? searchTerm,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new SearchCustomersQuery(tenantId, searchTerm), cancellationToken);
            return Results.Ok(response);
        })
        .WithTags("Customers");

        return app;
    }
}
