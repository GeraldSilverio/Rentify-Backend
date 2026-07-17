using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UploadCustomerDocument;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomerDocument;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Presentation.WebApi.Endpoints.Customers;

public static class CustomerDocumentsEndpoints
{
    public static IEndpointRouteBuilder MapCustomerDocumentsEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/v1/customers")
            .WithTags("Customers")
            .RequireAuthorization(AuthorizationPolicies.RequiredRoles);

        group.MapPost("/{customerId:guid}/documents", async (
            Guid customerId,
            IFormFile document,
            [FromForm] CustomerDocumentType documentType,
            [FromForm] DocumentSide documentSide,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new UploadCustomerDocumentCommand(
                currentRequestContext.TenantId,
                customerId,
                document,
                documentType,
                documentSide,
                currentRequestContext.UserId.ToString()), cancellationToken);

            return Results.Created($"/api/v1/customers/{customerId}/documents/{response.Value}", response);
        })
        .DisableAntiforgery();

        group.MapDelete("/{customerId:guid}/documents/{documentId:guid}", async (
            Guid customerId,
            Guid documentId,
            ICurrentRequestContext currentRequestContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            await sender.Send(new DeleteCustomerDocumentCommand(
                currentRequestContext.TenantId,
                customerId,
                documentId,
                currentRequestContext.UserId.ToString()), cancellationToken);

            return Results.NoContent();
        });

        return app;
    }
}
