using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Presentation.WebApi.Middlewares;
using Serilog;

namespace Rentify.Backend.Presentation.WebApi.Logging;

public static class HttpLogContextEnricher
{
    public static void Enrich(
        IDiagnosticContext diagnosticContext,
        HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(diagnosticContext);
        ArgumentNullException.ThrowIfNull(httpContext);

        diagnosticContext.Set("CorrelationId", GetCorrelationId(httpContext));
        diagnosticContext.Set("TraceIdentifier", httpContext.TraceIdentifier);
        diagnosticContext.Set("TraceId", Activity.Current?.TraceId.ToString());
        diagnosticContext.Set("ConnectionId", httpContext.Connection.Id);
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);

        Guid? tenantId = GetTenantId(httpContext.User);
        Guid? userId = GetUserId(httpContext.User);

        if (tenantId.HasValue)
            diagnosticContext.Set("TenantId", tenantId.Value);

        if (userId.HasValue)
            diagnosticContext.Set("UserId", userId.Value);
    }

    public static string GetCorrelationId(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        return httpContext.Items.TryGetValue(
                   CorrelationIdMiddleware.CorrelationIdItemKey,
                   out object? value)
               && value is string correlationId
            ? correlationId
            : httpContext.TraceIdentifier;
    }

    public static Guid? GetTenantId(ClaimsPrincipal user)
    {
        return GetGuidClaim(
            user,
            ApplicationClaimTypes.TenantId,
            "TenantId",
            "tenant_id");
    }

    public static Guid? GetUserId(ClaimsPrincipal user)
    {
        return GetGuidClaim(
            user,
            ApplicationClaimTypes.UserId,
            ClaimTypes.NameIdentifier,
            JwtRegisteredClaimNames.Sub,
            "sub");
    }

    private static Guid? GetGuidClaim(
        ClaimsPrincipal user,
        params string[] claimTypes)
    {
        string? value = claimTypes
            .Select(type => user.FindFirst(type)?.Value)
            .FirstOrDefault(candidate => !string.IsNullOrWhiteSpace(candidate));

        return Guid.TryParse(value, out Guid identifier) && identifier != Guid.Empty
            ? identifier
            : null;
    }
}
