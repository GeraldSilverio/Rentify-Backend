using MediatR;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantSettings;

public sealed record UpdateTenantSettingsCommand(
    Guid TenantId,
    string CurrencyCode,
    string TimeZone,
    bool EnableReservations,
    bool EnableDriverFleet,
    bool EnableMaintenance,
    bool EnableLateFees,
    bool EnablePublicCatalog,
    string ModifiedBy) : IRequest<ResultReponse<TenantSettingsResponse>>;
