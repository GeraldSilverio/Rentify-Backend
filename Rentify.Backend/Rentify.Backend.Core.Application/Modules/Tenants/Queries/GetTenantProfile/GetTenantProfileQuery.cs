using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantProfile;

public sealed record GetTenantProfileQuery(Guid TenantId) : IRequest<ResultReponse<TenantProfileResponse>>;
