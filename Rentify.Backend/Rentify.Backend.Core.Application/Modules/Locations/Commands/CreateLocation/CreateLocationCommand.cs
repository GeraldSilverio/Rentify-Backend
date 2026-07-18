using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.CreateLocation;

public sealed record CreateLocationCommand(
    string Name,
    LocationType Type,
    string? City,
    string? Province,
    string? Country,
    string? Address,
    string? Notes,
    string CreatedBy) : IRequest<ResultReponse<LocationResponse>>;
