using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantUsage;

public sealed record GetTenantUsageQuery(Guid TenantId) : IRequest<ResultReponse<TenantUsageResponse>>;
