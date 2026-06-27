using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantProfile;

public sealed record UpdateTenantProfileCommand(
    Guid TenantId,
    string Name,
    string? LegalName,
    string? Rnc,
    string ModifiedBy) : IRequest<ResultReponse<TenantProfileResponse>>;
