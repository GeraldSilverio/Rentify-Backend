using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;

namespace Rentify.Backend.Core.Application.Modules.Shared.Logging;

public static class RequestContextLogValues
{
    public static Guid? GetTenantId(ICurrentRequestContext currentRequestContext)
    {
        ArgumentNullException.ThrowIfNull(currentRequestContext);

        return currentRequestContext.TryGetTenantId(out Guid tenantId)
            ? tenantId
            : null;
    }

    public static Guid? GetUserId(ICurrentRequestContext currentRequestContext)
    {
        ArgumentNullException.ThrowIfNull(currentRequestContext);

        if (!currentRequestContext.IsAuthenticated)
            return null;

        try
        {
            return currentRequestContext.UserId;
        }
        catch (ApiException)
        {
            return null;
        }
    }
}
