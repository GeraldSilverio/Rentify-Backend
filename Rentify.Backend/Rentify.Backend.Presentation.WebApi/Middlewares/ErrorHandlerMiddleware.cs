using Microsoft.IdentityModel.Tokens;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using System.Net;
using System.Text.Json;

namespace Rentify.Backend.Presentation.WebApi.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                HttpResponse response = context.Response;

                if (response.HasStarted)
                {
                    throw;
                }

                response.ContentType = "application/json";
                response.StatusCode = GetStatusCode(error);

                Error responseError = CreateError(error, response.StatusCode);
                ResultReponse<string> responseModel = ResultReponse<string>.Failure(responseError);

                string result = JsonSerializer.Serialize(
                    responseModel,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                await response.WriteAsync(result);
            }
        }

        private static int GetStatusCode(Exception error)
        {
            return error switch
            {
                SecurityTokenExpiredException => (int)HttpStatusCode.Unauthorized,
                SecurityTokenException => (int)HttpStatusCode.Unauthorized,
                ApiException exception => exception.ErrorCode switch
                {
                    (int)HttpStatusCode.BadRequest => (int)HttpStatusCode.BadRequest,
                    (int)HttpStatusCode.InternalServerError => (int)HttpStatusCode.InternalServerError,
                    (int)HttpStatusCode.NotFound => (int)HttpStatusCode.NotFound,
                    (int)HttpStatusCode.NoContent => (int)HttpStatusCode.NoContent,
                    (int)HttpStatusCode.Unauthorized => (int)HttpStatusCode.Unauthorized,
                    (int)HttpStatusCode.Forbidden => (int)HttpStatusCode.Forbidden,
                    (int)HttpStatusCode.Conflict => (int)HttpStatusCode.Conflict,
                    (int)HttpStatusCode.BadGateway => (int)HttpStatusCode.BadGateway,
                    _ => (int)HttpStatusCode.InternalServerError
                },
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
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
                _ => Error.SetError(error.Message, statusCode)
            };
        }
    }
}
