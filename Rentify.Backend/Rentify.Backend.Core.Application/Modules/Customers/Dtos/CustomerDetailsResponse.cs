namespace Rentify.Backend.Core.Application.Modules.Customers.Dtos;

public sealed record CustomerDetailsResponse(
    Guid Id,
    Guid TenantId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    bool IsActive,
    DateTime CreatedDate,
    DateTime ModifiedDate,
    IReadOnlyList<CustomerDocumentResponse> Documents);
