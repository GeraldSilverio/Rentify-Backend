using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Customers.Commands.UploadCustomerDocument;

public sealed record UploadCustomerDocumentCommand(
    Guid TenantId,
    Guid CustomerId,
    IFormFile Document,
    CustomerDocumentType DocumentType,
    string CreatedBy) : IRequest<ResultReponse<Guid>>;
