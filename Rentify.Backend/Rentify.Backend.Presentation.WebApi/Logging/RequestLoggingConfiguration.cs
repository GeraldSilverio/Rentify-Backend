using Serilog.AspNetCore;
using Serilog.Events;

namespace Rentify.Backend.Presentation.WebApi.Logging;

public static class RequestLoggingConfiguration
{
    public static void Configure(RequestLoggingOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, _, exception) =>
        {
            if (exception is not null
                || httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError)
            {
                return LogEventLevel.Error;
            }

            return httpContext.Response.StatusCode >= StatusCodes.Status400BadRequest
                ? LogEventLevel.Warning
                : LogEventLevel.Information;
        };
        options.EnrichDiagnosticContext = HttpLogContextEnricher.Enrich;
    }
}
