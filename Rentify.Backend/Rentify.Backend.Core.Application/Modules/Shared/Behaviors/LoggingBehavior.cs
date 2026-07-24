using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Shared.Logging;

namespace Rentify.Backend.Core.Application.Modules.Shared.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentRequestContext _currentRequestContext;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentRequestContext currentRequestContext)
    {
        _logger = logger;
        _currentRequestContext = currentRequestContext;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        bool isCommand = requestName.EndsWith("Command", StringComparison.Ordinal);
        string? correlationId = Activity.Current?.GetTagItem("CorrelationId")?.ToString();
        Guid? tenantId = RequestContextLogValues.GetTenantId(_currentRequestContext);
        Guid? userId = RequestContextLogValues.GetUserId(_currentRequestContext);
        Stopwatch stopwatch = Stopwatch.StartNew();

        LogStarted(isCommand, requestName, correlationId, tenantId, userId);

        try
        {
            TResponse response = await next();
            stopwatch.Stop();

            if (IsFailedResponse(response))
            {
                _logger.LogWarning(
                    "Request {RequestName} returned a failed result in {ElapsedMilliseconds} ms with correlation {CorrelationId} for Tenant {TenantId} and User {UserId}",
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    correlationId,
                    tenantId,
                    userId);
            }
            else
            {
                LogCompleted(
                    isCommand,
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    correlationId,
                    tenantId,
                    userId);
            }

            return response;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                "Request {RequestName} failed after {ElapsedMilliseconds} ms with {ExceptionType} and correlation {CorrelationId} for Tenant {TenantId} and User {UserId}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                exception.GetType().Name,
                correlationId,
                tenantId,
                userId);

            throw;
        }
    }

    private void LogStarted(
        bool isCommand,
        string requestName,
        string? correlationId,
        Guid? tenantId,
        Guid? userId)
    {
        if (isCommand)
        {
            _logger.LogInformation(
                "Handling request {RequestName} with correlation {CorrelationId} for Tenant {TenantId} and User {UserId}",
                requestName,
                correlationId,
                tenantId,
                userId);
            return;
        }

        _logger.LogDebug(
            "Handling request {RequestName} with correlation {CorrelationId} for Tenant {TenantId} and User {UserId}",
            requestName,
            correlationId,
            tenantId,
            userId);
    }

    private void LogCompleted(
        bool isCommand,
        string requestName,
        long elapsedMilliseconds,
        string? correlationId,
        Guid? tenantId,
        Guid? userId)
    {
        if (isCommand)
        {
            _logger.LogInformation(
                "Handled request {RequestName} in {ElapsedMilliseconds} ms with correlation {CorrelationId} for Tenant {TenantId} and User {UserId}",
                requestName,
                elapsedMilliseconds,
                correlationId,
                tenantId,
                userId);
            return;
        }

        _logger.LogDebug(
            "Handled request {RequestName} in {ElapsedMilliseconds} ms with correlation {CorrelationId} for Tenant {TenantId} and User {UserId}",
            requestName,
            elapsedMilliseconds,
            correlationId,
            tenantId,
            userId);
    }

    private static bool IsFailedResponse(TResponse response)
    {
        if (response is null)
            return false;

        object? isSuccess = response
            .GetType()
            .GetProperty("IsSuccess")?
            .GetValue(response);

        return isSuccess is false;
    }
}
