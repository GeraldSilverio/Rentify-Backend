namespace Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

public sealed record TenantSettingsResponse(
    string CurrencyCode,
    string TimeZone,
    bool EnableReservations,
    bool EnableDriverFleet,
    bool EnableMaintenance,
    bool EnableLateFees,
    bool EnablePublicCatalog);
