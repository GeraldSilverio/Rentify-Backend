namespace Rentify.Backend.Core.Application.Modules.Shared.Context;

public interface ICurrentRequestContext
{
    bool IsAuthenticated { get; }

    Guid UserId { get; }

    Guid TenantId { get; }

    bool HasTenant { get; }

    string? UserName { get; }

    string? Email { get; }

    IReadOnlyCollection<string> Roles { get; }

    bool IsSuperAdmin { get; }

    string ModifiedBy { get; }

    bool TryGetTenantId(out Guid tenantId);
}
