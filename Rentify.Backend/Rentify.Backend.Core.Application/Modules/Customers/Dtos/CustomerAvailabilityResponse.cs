namespace Rentify.Backend.Core.Application.Modules.Customers.Dtos;

public sealed record CustomerAvailabilityResponse(
    bool Available,
    string? Message);
