using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAvailableLocations;

public sealed record GetAvailableLocationsQuery(
    string? Search = null,
    LocationType? Type = null) : IRequest<ResultReponse<IReadOnlyCollection<AvailableLocationResponse>>>;
