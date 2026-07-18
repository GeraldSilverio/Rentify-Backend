namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.CreateTenantLocation;

public sealed record CreateTenantLocationRequest(
    Guid? LocationId,
    string? DisplayName,
    bool AllowsDelivery,
    bool AllowsPickup,
    decimal DeliveryFee,
    decimal PickupFee,
    bool IsCustom);
