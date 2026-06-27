namespace Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

public sealed record TenantUsageResponse(
    int VehiclesUsed,
    int MaxVehicles,
    int UsersUsed,
    int MaxEmployees,
    int BranchesUsed,
    int MaxBranches,
    int ReservationsThisMonth,
    int MaxReservationsPerMonth);
