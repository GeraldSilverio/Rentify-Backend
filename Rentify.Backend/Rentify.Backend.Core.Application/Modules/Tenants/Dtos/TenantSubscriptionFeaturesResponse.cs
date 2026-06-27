namespace Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

public sealed record TenantSubscriptionFeaturesResponse(
    bool MultiBranchEnabled,
    bool ReportsEnabled,
    bool ApiAccessEnabled,
    bool ContractsEnabled,
    bool MaintenanceModuleEnabled,
    bool PrioritySupportEnabled,
    bool WhiteLabelEnabled);
