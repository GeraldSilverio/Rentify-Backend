using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenants;

public sealed record GetAdminTenantsQuery(
    string? Search = null,
    BusinessModel? BusinessModel = null,
    bool? IsActive = null,
    SubscriptionStatus? SubscriptionStatus = null,
    string? PlanCode = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<ResultReponse<PaginatedResponse<AdminTenantListItemResponse>>>;
