namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantProfile;

public sealed record UpdateTenantProfileRequest(
    string Name,
    string? LegalName,
    string? Rnc);
