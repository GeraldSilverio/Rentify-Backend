using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Customers.Dtos;

public sealed record CustomerDocumentResponse(
    Guid Id,
    string Name,
    string Url,
    string PublicId,
    CustomerDocumentType DocumentType,
    DocumentSide DocumentSide,
    DateTime CreatedDate);
