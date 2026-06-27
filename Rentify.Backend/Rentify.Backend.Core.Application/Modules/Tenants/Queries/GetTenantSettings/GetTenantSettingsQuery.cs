using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Queries.GetTenantSettings;

public sealed record GetTenantSettingsQuery(Guid TenantId) : IRequest<ResultReponse<TenantSettingsResponse>>;
