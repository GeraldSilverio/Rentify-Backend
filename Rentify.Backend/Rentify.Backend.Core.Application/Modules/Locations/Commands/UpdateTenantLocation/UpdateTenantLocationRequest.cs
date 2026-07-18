namespace Rentify.Backend.Core.Application.Modules.Locations.Commands.UpdateTenantLocation;

public sealed record UpdateTenantLocationRequest(
    string DisplayName,
    bool AllowsDelivery,
    bool AllowsPickup,
    decimal DeliveryFee,
    decimal PickupFee);
