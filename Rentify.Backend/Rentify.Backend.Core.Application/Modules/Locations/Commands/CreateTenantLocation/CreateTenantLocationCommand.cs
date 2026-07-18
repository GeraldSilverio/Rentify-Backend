using MediatR;
using Rentify.Backend.Core.Application.Modules.Locations.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.CreateTenantLocation;

public sealed record CreateTenantLocationCommand(
    Guid TenantId,
    Guid? LocationId,
    string? DisplayName,
    bool AllowsDelivery,
    bool AllowsPickup,
    decimal DeliveryFee,
    decimal PickupFee,
    bool IsCustom,
    string CreatedBy) : IRequest<ResultReponse<TenantLocationResponse>>;
