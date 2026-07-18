using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;

public interface ITenantUniquenessService
{
    Task ValidateUniqueFieldsAsync(
        RegisterTenantCommand command,
        Guid? excludedTenantId = null,
        CancellationToken cancellationToken = default);
}
