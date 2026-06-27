using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.DeactivateAdminTenant;

public sealed record DeactivateAdminTenantCommand(
    Guid TenantId,
    string ModifiedBy) : IRequest<ResultReponse<AdminTenantDetailResponse>>;
