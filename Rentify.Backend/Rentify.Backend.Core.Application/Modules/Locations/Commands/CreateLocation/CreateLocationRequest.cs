using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.CreateLocation;

public sealed record CreateLocationRequest(
    string Name,
    LocationType Type,
    string? City,
    string? Province,
    string? Country,
    string? Address,
    string? Notes);
