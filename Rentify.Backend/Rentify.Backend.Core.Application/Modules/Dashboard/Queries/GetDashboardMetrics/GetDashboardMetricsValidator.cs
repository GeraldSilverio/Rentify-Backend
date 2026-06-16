using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Dashboard.Queries.GetDashboardMetrics;

public sealed class GetDashboardMetricsValidator : AbstractValidator<GetDashboardMetricsQuery>
{
    public GetDashboardMetricsValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
    }
}
