namespace Rentify.Backend.Core.Application.Modules.Customers.Dtos;

public sealed record UnverifyCustomerResponse(
    Guid CustomerId,
    bool IsVerified,
    DateTime? VerifiedAt,
    string? VerifiedBy);
