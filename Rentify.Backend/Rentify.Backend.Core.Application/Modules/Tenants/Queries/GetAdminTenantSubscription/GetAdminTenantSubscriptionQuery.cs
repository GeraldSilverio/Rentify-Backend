using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenantSubscription;

public sealed record GetAdminTenantSubscriptionQuery(Guid TenantId) : IRequest<ResultReponse<TenantSubscriptionResponse>>;
