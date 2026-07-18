using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Locations.Dtos;

public sealed record AvailableLocationResponse(
    Guid Id,
    string Name,
    LocationType Type,
    string? City,
    string? Province,
    string Country,
    string? Address);
