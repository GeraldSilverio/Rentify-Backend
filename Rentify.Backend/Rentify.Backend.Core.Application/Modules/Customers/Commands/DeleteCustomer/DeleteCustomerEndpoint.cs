using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomer;

public static class DeleteCustomerEndpoint
{
    public static IEndpointRouteBuilder MapDeleteCustomerEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/tenants/{tenantId:guid}/customers/{customerId:guid}", async (
            Guid tenantId,
            Guid customerId,
            string modifiedBy,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new DeleteCustomerCommand(tenantId, customerId, modifiedBy), cancellationToken);
            return Results.Ok(response);
        })
        .WithTags("Customers");

        return app;
    }
}
