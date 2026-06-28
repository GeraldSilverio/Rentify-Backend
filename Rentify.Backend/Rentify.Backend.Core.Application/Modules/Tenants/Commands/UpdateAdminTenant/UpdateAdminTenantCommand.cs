using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateAdminTenant;

public sealed record UpdateAdminTenantCommand(
    Guid TenantId,
    string Name,
    string? LegalName,
    string? Rnc,
    BusinessModel? BusinessModel,
    string ModifiedBy) : IRequest<ResultReponse<AdminTenantDetailResponse>>;
