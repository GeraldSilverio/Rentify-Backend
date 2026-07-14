using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Queries.SearchCustomers;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Customers;

public static class CustomersEndpoints
{
    public static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/v1/customers")
            .WithTags("Customers")
            .RequireAuthorization(AuthorizationPolicies.RequiredRoles);

        group.MapPost("/", async (
            CreateCustomerRequest request,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new CreateCustomerCommand(
                currentRequestContext.TenantId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                currentRequestContext.ModifiedBy), cancellationToken);

            return Results.Created($"/api/v1/customers/{response.Value}", response);
        });

        group.MapPut("/{customerId:guid}", async (
            Guid customerId,
            UpdateCustomerRequest request,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new UpdateCustomerCommand(
                currentRequestContext.TenantId,
                customerId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                currentRequestContext.ModifiedBy), cancellationToken);

            return Results.Ok(response);
        });

        group.MapDelete("/{customerId:guid}", async (
            Guid customerId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new DeleteCustomerCommand(
                currentRequestContext.TenantId,
                customerId,
                currentRequestContext.ModifiedBy), cancellationToken);

            return Results.Ok(response);
        });

        group.MapGet("/", async (
            string? searchTerm,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new SearchCustomersQuery(
                currentRequestContext.TenantId,
                searchTerm), cancellationToken);

            return Results.Ok(response);
        });

        return app;
    }
}
