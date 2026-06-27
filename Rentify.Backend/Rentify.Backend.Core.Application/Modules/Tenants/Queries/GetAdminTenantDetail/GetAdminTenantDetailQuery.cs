using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetAdminTenantDetail;

public sealed record GetAdminTenantDetailQuery(Guid TenantId) : IRequest<ResultReponse<AdminTenantDetailResponse>>;
