using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.ActivateTenantLocation;

public sealed record ActivateTenantLocationCommand(
    Guid TenantId,
    Guid TenantLocationId,
    string ModifiedBy) : IRequest<ResultReponse<bool>>;
