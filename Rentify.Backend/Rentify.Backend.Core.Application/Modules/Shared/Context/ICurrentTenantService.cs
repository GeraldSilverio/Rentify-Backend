namespace Rentify.Backend.Core.Application.Modules.Shared.Context;

public interface ICurrentTenantService
{
    Guid TenantId { get; }

    bool HasTenant { get; }

    Guid GetTenantId();
}
