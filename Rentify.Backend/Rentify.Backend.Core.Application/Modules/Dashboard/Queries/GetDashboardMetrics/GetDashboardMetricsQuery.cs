using MediatR;
using Rentify.Backend.Core.Application.Modules.Dashboard.Dtos;
using Rentify.Backend.Core.Application.Shared.Response;

namespace Rentify.Backend.Core.Application.Modules.Dashboard.Queries.GetDashboardMetrics;

public sealed record GetDashboardMetricsQuery(Guid TenantId) : IRequest<ResultReponse<DashboardMetricsResponse>>;
