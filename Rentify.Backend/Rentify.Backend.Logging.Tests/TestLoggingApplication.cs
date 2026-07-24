using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Presentation.WebApi.Logging;
using Rentify.Backend.Presentation.WebApi.Middlewares;
using Serilog;
using Serilog.Core;

namespace Rentify.Backend.Logging.Tests;

internal sealed class TestLoggingApplication : IAsyncDisposable
{
    private TestLoggingApplication(
        WebApplication application,
        HttpClient client,
        Logger logger,
        TestLogSink sink)
    {
        Application = application;
        Client = client;
        Logger = logger;
        Sink = sink;
    }

    public WebApplication Application { get; }

    public HttpClient Client { get; }

    public Logger Logger { get; }

    public TestLogSink Sink { get; }

    public static async Task<TestLoggingApplication> StartAsync()
    {
        TestLogSink sink = new();
        Logger logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Sink(sink)
            .CreateLogger();
        Log.Logger = logger;

        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Testing"
        });

        builder.WebHost.UseTestServer();
        builder.Services.AddSerilog(logger, dispose: false);

        WebApplication application = builder.Build();

        application.UseMiddleware<CorrelationIdMiddleware>();
        application.UseSerilogRequestLogging(RequestLoggingConfiguration.Configure);
        application.UseMiddleware<ErrorHandlerMiddleware>();
        application.Use(async (context, next) =>
        {
            if (context.Request.Path == "/authenticated")
            {
                context.User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ApplicationClaimTypes.UserId, TestIdentifiers.UserId.ToString()),
                    new Claim(ApplicationClaimTypes.TenantId, TestIdentifiers.TenantId.ToString())
                ], "TestAuthentication"));
            }

            await next(context);
        });

        application.Run(context => context.Request.Path.Value switch
        {
            "/bad-request" => SetStatusAsync(context, StatusCodes.Status400BadRequest),
            "/failure" => throw new InvalidOperationException("Synthetic test failure"),
            _ => SetStatusAsync(context, StatusCodes.Status200OK)
        });

        await application.StartAsync();

        return new TestLoggingApplication(
            application,
            application.GetTestClient(),
            logger,
            sink);
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await Application.StopAsync();
        await Application.DisposeAsync();
        Log.Logger = new LoggerConfiguration().CreateLogger();
        Logger.Dispose();
    }

    private static Task SetStatusAsync(HttpContext context, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        return Task.CompletedTask;
    }
}

internal static class TestIdentifiers
{
    public static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid TenantId = Guid.Parse("22222222-2222-2222-2222-222222222222");
}
