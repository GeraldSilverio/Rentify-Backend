using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Locations.Dtos;

public sealed record TenantLocationListItemResponse(
    Guid Id,
    Guid? LocationId,
    string DisplayName,
    LocationType? LocationType,
    string? City,
    string? Province,
    bool AllowsDelivery,
    bool AllowsPickup,
    decimal DeliveryFee,
    decimal PickupFee,
    bool IsCustom,
    bool IsActive);
