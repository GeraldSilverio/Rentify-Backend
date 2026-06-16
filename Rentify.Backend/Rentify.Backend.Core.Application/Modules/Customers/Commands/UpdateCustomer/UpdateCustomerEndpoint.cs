using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;

public static class UpdateCustomerEndpoint
{
    public static IEndpointRouteBuilder MapUpdateCustomerEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/tenants/{tenantId:guid}/customers/{customerId:guid}", async (
            Guid tenantId,
            Guid customerId,
            UpdateCustomerRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new UpdateCustomerCommand(
                tenantId,
                customerId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.LicenseNumber,
                request.LicenseExpirationDate,
                request.ModifiedBy), cancellationToken);

            return Results.Ok(response);
        })
        .WithTags("Customers");

        return app;
    }
}
