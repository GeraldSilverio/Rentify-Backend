using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.UpdateLocation;

public sealed record UpdateLocationCommand(
    Guid LocationId,
    string Name,
    LocationType Type,
    string? City,
    string? Province,
    string? Country,
    string? Address,
    string? Notes,
    string ModifiedBy) : IRequest<ResultReponse<LocationResponse>>;
