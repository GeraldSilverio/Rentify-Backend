using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.DeleteTenantLocation;

public sealed record DeleteTenantLocationCommand(
    Guid TenantId,
    Guid TenantLocationId,
    string ModifiedBy) : IRequest<ResultReponse<bool>>;
