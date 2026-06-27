namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantSettings;

public sealed record UpdateTenantSettingsRequest(
    string CurrencyCode,
    string TimeZone,
    bool EnableReservations,
    bool EnableDriverFleet,
    bool EnableMaintenance,
    bool EnableLateFees,
    bool EnablePublicCatalog);
