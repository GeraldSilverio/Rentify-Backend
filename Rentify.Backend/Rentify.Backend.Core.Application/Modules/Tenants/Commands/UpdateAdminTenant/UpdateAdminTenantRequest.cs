using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateAdminTenant;

public sealed record UpdateAdminTenantRequest(
    string Name,
    string? LegalName,
    string? Rnc,
    BusinessModel? BusinessModel);
