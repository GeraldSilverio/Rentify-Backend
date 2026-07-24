using System.Diagnostics;
using Serilog.Context;

namespace Rentify.Backend.Presentation.WebApi.Middlewares;

public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID";
    public const string CorrelationIdItemKey = "Rentify.CorrelationId";
    public const int MaximumCorrelationIdLength = 128;

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId = ResolveCorrelationId(context.Request.Headers[HeaderName]);
        context.Items[CorrelationIdItemKey] = correlationId;

        context.Response.OnStarting(static state =>
        {
            CorrelationResponseState responseState = (CorrelationResponseState)state;
            responseState.Context.Response.Headers[HeaderName] = responseState.CorrelationId;
            return Task.CompletedTask;
        }, new CorrelationResponseState(context, correlationId));

        string? traceId = Activity.Current?.TraceId.ToString();

        using IDisposable correlationScope = LogContext.PushProperty("CorrelationId", correlationId);
        using IDisposable traceIdentifierScope = LogContext.PushProperty("TraceIdentifier", context.TraceIdentifier);
        using IDisposable traceIdScope = LogContext.PushProperty("TraceId", traceId);
        using IDisposable connectionScope = LogContext.PushProperty("ConnectionId", context.Connection.Id);

        _logger.LogInformation(
            "HTTP request started {RequestMethod} {RequestPath}",
            context.Request.Method,
            context.Request.Path.Value ?? "/");

        await _next(context);
    }

    public static string ResolveCorrelationId(string? candidate)
    {
        return IsValid(candidate)
            ? candidate!
            : Guid.NewGuid().ToString("N");
    }

    public static bool IsValid(string? candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate)
            || candidate.Length > MaximumCorrelationIdLength)
        {
            return false;
        }

        return candidate.All(character =>
            char.IsAsciiLetterOrDigit(character)
            || character is '-' or '_' or '.' or ':');
    }

    private sealed record CorrelationResponseState(
        HttpContext Context,
        string CorrelationId);
}
