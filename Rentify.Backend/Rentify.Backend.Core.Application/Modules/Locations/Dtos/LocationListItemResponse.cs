using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Locations.Dtos;

public sealed record LocationListItemResponse(
    Guid Id,
    string Name,
    LocationType Type,
    string? City,
    string? Province,
    string Country,
    string? Address,
    bool IsActive);
