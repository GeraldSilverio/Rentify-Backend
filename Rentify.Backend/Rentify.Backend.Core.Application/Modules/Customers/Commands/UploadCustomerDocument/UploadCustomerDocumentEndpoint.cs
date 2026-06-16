using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UploadCustomerDocument;

public static class UploadCustomerDocumentEndpoint
{
    public static IEndpointRouteBuilder MapUploadCustomerDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/tenants/{tenantId:guid}/customers/{customerId:guid}/documents", async (
            Guid tenantId,
            Guid customerId,
            IFormFile document,
            [FromForm] CustomerDocumentType documentType,
            [FromForm] string createdBy,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new UploadCustomerDocumentCommand(
                tenantId,
                customerId,
                document,
                documentType,
                createdBy), cancellationToken);

            return Results.Created($"/api/v1/tenants/{tenantId}/customers/{customerId}/documents/{response.Value}", response);
        })
        .DisableAntiforgery()
        .WithTags("Customers");

        return app;
    }
}
