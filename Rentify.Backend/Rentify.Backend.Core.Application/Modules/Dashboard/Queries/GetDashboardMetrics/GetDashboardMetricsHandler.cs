using MediatR;
using Rentify.Backend.Core.Application.Modules.Dashboard.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Dashboard.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Dashboard.Queries.GetDashboardMetrics;

public sealed class GetDashboardMetricsHandler : IRequestHandler<GetDashboardMetricsQuery, ResultReponse<DashboardMetricsResponse>>
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetDashboardMetricsHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<ResultReponse<DashboardMetricsResponse>> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        DashboardMetricsResponse metrics = await _dashboardRepository.GetMetricsAsync(
            request.TenantId,
            DateOnly.FromDateTime(DateTime.UtcNow),
            cancellationToken);

        return ResultReponse<DashboardMetricsResponse>.Success(metrics);
    }
}
