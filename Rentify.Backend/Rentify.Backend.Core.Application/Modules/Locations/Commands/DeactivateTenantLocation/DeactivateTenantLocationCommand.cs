using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.DeactivateTenantLocation;

public sealed record DeactivateTenantLocationCommand(
    Guid TenantId,
    Guid TenantLocationId,
    string ModifiedBy) : IRequest<ResultReponse<bool>>;
