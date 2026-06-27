namespace Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

public sealed record TenantSubscriptionLimitsResponse(
    int MaxVehicles,
    int MaxEmployees,
    int MaxBranches,
    int MaxReservationsPerMonth);
