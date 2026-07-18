using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetTenantLocations;

public sealed record GetTenantLocationsQuery(
    Guid TenantId,
    string? Search = null,
    bool? IsActive = null,
    bool? SupportsDelivery = null,
    bool? SupportsPickup = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<ResultReponse<PaginatedResponse<TenantLocationListItemResponse>>>;
