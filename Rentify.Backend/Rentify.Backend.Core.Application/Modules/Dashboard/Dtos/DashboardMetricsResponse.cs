namespace Rentify.Backend.Core.Application.Modules.Dashboard.Dtos;

public sealed record DashboardMetricsResponse(
    decimal MonthlyRevenue,
    int ActiveReservations,
    int AvailableVehicles,
    int UpcomingReturns);
