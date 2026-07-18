using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetTenantLocationById;

public sealed record GetTenantLocationByIdQuery(
    Guid TenantId,
    Guid TenantLocationId) : IRequest<ResultReponse<TenantLocationResponse>>;
