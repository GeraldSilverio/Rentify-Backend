using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.UpdateLocation;

public sealed record UpdateLocationRequest(
    string Name,
    LocationType Type,
    string? City,
    string? Province,
    string? Country,
    string? Address,
    string? Notes);
