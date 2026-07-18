using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAdminLocations;

public sealed record GetAdminLocationsQuery(
    string? Search = null,
    LocationType? Type = null,
    bool? IsActive = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<ResultReponse<PaginatedResponse<LocationListItemResponse>>>;
