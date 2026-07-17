namespace Rentify.Backend.Core.Application.Modules.Customers.Dtos;

public sealed record VerifyCustomerResponse(
    Guid CustomerId,
    bool IsVerified,
    DateTime? VerifiedAt,
    string? VerifiedBy);
