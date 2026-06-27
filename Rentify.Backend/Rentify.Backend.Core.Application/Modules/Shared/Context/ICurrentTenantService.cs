namespace Rentify.Backend.Core.Application.Modules.Shared.Context;

public interface ICurrentTenantService
{
    Guid GetTenantId();
}
