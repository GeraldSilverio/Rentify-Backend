using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.UpdateTenantLocation;

public sealed record UpdateTenantLocationCommand(
    Guid TenantId,
    Guid TenantLocationId,
    string DisplayName,
    bool AllowsDelivery,
    bool AllowsPickup,
    decimal DeliveryFee,
    decimal PickupFee,
    string ModifiedBy) : IRequest<ResultReponse<TenantLocationResponse>>;
