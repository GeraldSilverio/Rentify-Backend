using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;

public static class CreateCustomerEndpoint
{
    public static IEndpointRouteBuilder MapCreateCustomerEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/tenants/{tenantId:guid}/customers", async (
            Guid tenantId,
            CreateCustomerRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new CreateCustomerCommand(
                tenantId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.LicenseNumber,
                request.LicenseExpirationDate,
                request.CreatedBy), cancellationToken);

            return Results.Created($"/api/v1/tenants/{tenantId}/customers/{response.Value}", response);
        })
        .WithTags("Customers");

        return app;
    }
}
