using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Queries.GetAdminLocationById;

public sealed record GetAdminLocationByIdQuery(Guid LocationId) : IRequest<ResultReponse<LocationResponse>>;
