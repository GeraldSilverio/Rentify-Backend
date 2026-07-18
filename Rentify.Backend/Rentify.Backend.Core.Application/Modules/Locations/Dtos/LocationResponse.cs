using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Locations.Dtos;

public sealed record LocationResponse(
    Guid Id,
    string Name,
    LocationType Type,
    string? City,
    string? Province,
    string Country,
    string? Address,
    string? Notes,
    bool IsActive,
    DateTime? CreatedDate = null,
    DateTime? ModifiedDate = null);
