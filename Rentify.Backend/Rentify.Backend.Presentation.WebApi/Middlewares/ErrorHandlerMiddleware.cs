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
                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = error switch
                {
                    ApiException e => e.ErrorCode switch
                    {
                        (int)HttpStatusCode.BadRequest => (int)HttpStatusCode.BadRequest,
                        (int)HttpStatusCode.InternalServerError => (int)HttpStatusCode.InternalServerError,
                        (int)HttpStatusCode.NotFound => (int)HttpStatusCode.NotFound,
                        (int)HttpStatusCode.NoContent => (int)HttpStatusCode.NoContent,
                        (int)HttpStatusCode.Unauthorized => (int)HttpStatusCode.Unauthorized,
                        (int)HttpStatusCode.Conflict => (int)HttpStatusCode.Conflict,
                        (int)HttpStatusCode.BadGateway => (int)HttpStatusCode.BadGateway,
                        _ => (int)HttpStatusCode.InternalServerError,
                    },
                    KeyNotFoundException e => (int)HttpStatusCode.NotFound,
                    _ => (int)HttpStatusCode.InternalServerError,
                };
                var responseModel = ResultReponse<string>.Failure(Error.SetError(error.Message, response.StatusCode));

                var result = JsonSerializer.Serialize(responseModel.Error);

                await response.WriteAsync(result);
            }
        }
    }
}
