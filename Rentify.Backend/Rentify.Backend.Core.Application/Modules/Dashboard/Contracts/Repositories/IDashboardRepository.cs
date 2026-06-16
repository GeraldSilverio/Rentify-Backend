using Rentify.Backend.Core.Application.Modules.Dashboard.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Dashboard.Contracts.Repositories;

public interface IDashboardRepository
{
    Task<DashboardMetricsResponse> GetMetricsAsync(Guid tenantId, DateOnly today, CancellationToken cancellationToken = default);
}
