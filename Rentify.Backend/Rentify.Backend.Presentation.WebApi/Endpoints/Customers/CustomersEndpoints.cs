using MediatR;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.VerifyCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UnverifyCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Queries.GetCustomerById;
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
                request.CustomerType,
                request.IdentificationType,
                request.IdentificationNumber,
                request.BirthDate,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.AddressLine,
                request.Sector,
                request.City,
                request.Province,
                request.AddressReference,
                currentRequestContext.UserId.ToString()), cancellationToken);

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
                request.CustomerType,
                request.IdentificationType,
                request.IdentificationNumber,
                request.BirthDate,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.AddressLine,
                request.Sector,
                request.City,
                request.Province,
                request.AddressReference,
                currentRequestContext.ModifiedBy), cancellationToken);

            return Results.Ok(response);
        });

        group.MapPut("/{customerId:guid}/verify", async (
            Guid customerId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new VerifyCustomerCommand(
                currentRequestContext.TenantId,
                customerId,
                currentRequestContext.ModifiedBy), cancellationToken);

            return Results.Ok(response);
        });

        group.MapPut("/{customerId:guid}/unverify", async (
            Guid customerId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new UnverifyCustomerCommand(
                currentRequestContext.TenantId,
                customerId,
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

        group.MapGet("/{customerId:guid}", async (
            Guid customerId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetCustomerByIdQuery(
                currentRequestContext.TenantId,
                customerId), cancellationToken);

            return Results.Ok(response);
        });

        group.MapGet("/", async (
            string? searchTerm,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var response = await sender.Send(new SearchCustomersQuery(
                currentRequestContext.TenantId,
                pageNumber,
                pageSize,
                searchTerm), cancellationToken);

            return Results.Ok(response);
        });

        return app;
    }
}
