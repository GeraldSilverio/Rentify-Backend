using System.Net;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Presentation.WebApi.Logging;

namespace Rentify.Backend.Presentation.WebApi.Middlewares;

public sealed class ErrorHandlerMiddleware
{
    private const string UnexpectedErrorMessage = "An unexpected error occurred.";

    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            if (context.Response.HasStarted)
                throw;

            int statusCode = GetStatusCode(error);
            LogException(context, error, statusCode);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            Error responseError = CreateError(error, statusCode);
            ResultReponse<string> responseModel = ResultReponse<string>.Failure(responseError);

            string result = JsonSerializer.Serialize(
                responseModel,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            await context.Response.WriteAsync(result, context.RequestAborted);
        }
    }

    private void LogException(
        HttpContext context,
        Exception error,
        int statusCode)
    {
        Guid? tenantId = HttpLogContextEnricher.GetTenantId(context.User);
        Guid? userId = HttpLogContextEnricher.GetUserId(context.User);
        string correlationId = HttpLogContextEnricher.GetCorrelationId(context);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                error,
                "Unhandled exception processing {RequestMethod} {RequestPath}",
                context.Request.Method,
                context.Request.Path.Value ?? "/");
            return;
        }

        _logger.LogWarning(
            "Controlled error {ExceptionType} processing {RequestMethod} {RequestPath} with status {StatusCode} and correlation {CorrelationId} for Tenant {TenantId} and User {UserId}",
            error.GetType().Name,
            context.Request.Method,
            context.Request.Path.Value ?? "/",
            statusCode,
            correlationId,
            tenantId,
            userId);
    }

    private static int GetStatusCode(Exception error)
    {
        return error switch
        {
            SecurityTokenExpiredException => StatusCodes.Status401Unauthorized,
            SecurityTokenException => StatusCodes.Status401Unauthorized,
            ApiException exception => exception.ErrorCode switch
            {
                StatusCodes.Status400BadRequest => StatusCodes.Status400BadRequest,
                StatusCodes.Status500InternalServerError => StatusCodes.Status500InternalServerError,
                StatusCodes.Status404NotFound => StatusCodes.Status404NotFound,
                StatusCodes.Status204NoContent => StatusCodes.Status204NoContent,
                StatusCodes.Status401Unauthorized => StatusCodes.Status401Unauthorized,
                StatusCodes.Status403Forbidden => StatusCodes.Status403Forbidden,
                StatusCodes.Status409Conflict => StatusCodes.Status409Conflict,
                StatusCodes.Status502BadGateway => StatusCodes.Status502BadGateway,
                _ => StatusCodes.Status500InternalServerError
            },
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static Error CreateError(
        Exception error,
        int statusCode)
    {
        return error switch
        {
            SecurityTokenExpiredException => Error.SetError(
                "Tu sesión ha vencido.",
                statusCode,
                "TOKEN_EXPIRED"),
            SecurityTokenException => Error.SetError(
                "El token de acceso no es válido.",
                statusCode,
                "INVALID_TOKEN"),
            ApiException apiException => Error.SetError(
                apiException.Message,
                statusCode,
                apiException.Key),
            KeyNotFoundException => Error.SetError(
                error.Message,
                statusCode),
            _ => Error.SetError(UnexpectedErrorMessage, statusCode)
        };
    }
}
