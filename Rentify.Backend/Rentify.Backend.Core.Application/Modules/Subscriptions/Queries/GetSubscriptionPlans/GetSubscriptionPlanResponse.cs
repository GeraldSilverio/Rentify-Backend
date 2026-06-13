namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetSubscriptionPlans
{
    public record GetSubscriptionPlanResponse(
         Guid Id,
         string Name,
         decimal Price,
         int MaxVehicles,
         int MaxEmployees,
         int MaxBranches,
         int MaxReservtionPerMonth,
         bool MultiBranchEnabled,
         bool ReportsEnabled,
         bool ContractsEnabled,
         bool MaintenanceModuleEnabled,
         bool PrioritySupportEnabled,
         bool WhiteLabelEnabled);
}
