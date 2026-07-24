using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Serilog.Events;

namespace Rentify.Backend.Logging.Tests;

public sealed class HttpLoggingTests
{
    private const string CompletionTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

    [Fact]
    public async Task SuccessfulRequestWritesStartAndCompletionWithRequiredProperties()
    {
        await using TestLoggingApplication application = await TestLoggingApplication.StartAsync();

        using HttpResponseMessage response = await application.Client.GetAsync("/success");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        LogEvent started = Assert.Single(application.Sink.Events, logEvent =>
            logEvent.MessageTemplate.Text == "HTTP request started {RequestMethod} {RequestPath}");
        LogEvent completed = Assert.Single(application.Sink.Events, logEvent =>
            logEvent.MessageTemplate.Text == CompletionTemplate);

        Assert.Equal(LogEventLevel.Information, started.Level);
        Assert.Equal(LogEventLevel.Information, completed.Level);
        AssertProperty(started, "RequestMethod", "GET");
        AssertProperty(started, "RequestPath", "/success");
        Assert.True(started.Properties.ContainsKey("RequestHost"));
        Assert.True(started.Properties.ContainsKey("RequestScheme"));
        Assert.True(started.Properties.ContainsKey("RequestProtocol"));
        AssertProperty(completed, "RequestMethod", "GET");
        AssertProperty(completed, "RequestPath", "/success");
        AssertProperty(completed, "StatusCode", 200);
        Assert.True(completed.Properties.ContainsKey("Elapsed"));
        Assert.True(completed.Properties.ContainsKey("CorrelationId"));
    }

    [Fact]
    public async Task BadRequestIsLoggedAsWarning()
    {
        await using TestLoggingApplication application = await TestLoggingApplication.StartAsync();

        using HttpResponseMessage response = await application.Client.GetAsync("/bad-request");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        LogEvent completed = Assert.Single(application.Sink.Events, logEvent =>
            logEvent.MessageTemplate.Text == CompletionTemplate);
        Assert.Equal(LogEventLevel.Warning, completed.Level);
    }

    [Fact]
    public async Task UnhandledExceptionIsLoggedOnceWithStackAndCompletionIsError()
    {
        await using TestLoggingApplication application = await TestLoggingApplication.StartAsync();

        using HttpResponseMessage response = await application.Client.GetAsync("/failure");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        LogEvent exceptionEvent = Assert.Single(application.Sink.Events, logEvent =>
            logEvent.MessageTemplate.Text == "Unhandled exception processing {RequestMethod} {RequestPath}");
        LogEvent completed = Assert.Single(application.Sink.Events, logEvent =>
            logEvent.MessageTemplate.Text == CompletionTemplate);

        Assert.Equal(LogEventLevel.Error, exceptionEvent.Level);
        Assert.NotNull(exceptionEvent.Exception);
        Assert.False(string.IsNullOrWhiteSpace(exceptionEvent.Exception.StackTrace));
        Assert.Equal(LogEventLevel.Error, completed.Level);
        Assert.Null(completed.Exception);
    }

    [Fact]
    public async Task ResponsePreservesValidIncomingCorrelationId()
    {
        await using TestLoggingApplication application = await TestLoggingApplication.StartAsync();
        using HttpRequestMessage request = new(HttpMethod.Get, "/success");
        request.Headers.Add("X-Correlation-ID", "rentify-test_123:abc");

        using HttpResponseMessage response = await application.Client.SendAsync(request);

        Assert.Equal("rentify-test_123:abc", response.Headers.GetValues("X-Correlation-ID").Single());
        LogEvent completed = Assert.Single(application.Sink.Events, logEvent =>
            logEvent.MessageTemplate.Text == CompletionTemplate);
        AssertProperty(completed, "CorrelationId", "rentify-test_123:abc");
    }

    [Theory]
    [InlineData("invalid correlation id")]
    [InlineData("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")]
    public async Task InvalidIncomingCorrelationIdIsReplaced(string invalidCorrelationId)
    {
        await using TestLoggingApplication application = await TestLoggingApplication.StartAsync();
        using HttpRequestMessage request = new(HttpMethod.Get, "/success");
        request.Headers.Add("X-Correlation-ID", invalidCorrelationId);

        using HttpResponseMessage response = await application.Client.SendAsync(request);

        string responseCorrelationId = response.Headers.GetValues("X-Correlation-ID").Single();
        Assert.NotEqual(invalidCorrelationId, responseCorrelationId);
        Assert.True(Rentify.Backend.Presentation.WebApi.Middlewares.CorrelationIdMiddleware.IsValid(responseCorrelationId));
    }

    [Fact]
    public async Task CompletionIncludesTenantAndUserForAuthenticatedRequest()
    {
        await using TestLoggingApplication application = await TestLoggingApplication.StartAsync();

        using HttpResponseMessage response = await application.Client.GetAsync("/authenticated");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        LogEvent completed = Assert.Single(application.Sink.Events, logEvent =>
            logEvent.MessageTemplate.Text == CompletionTemplate);
        AssertProperty(completed, "TenantId", TestIdentifiers.TenantId);
        AssertProperty(completed, "UserId", TestIdentifiers.UserId);
    }

    [Fact]
    public async Task PublicRequestDoesNotRequireTenantOrUser()
    {
        await using TestLoggingApplication application = await TestLoggingApplication.StartAsync();

        using HttpResponseMessage response = await application.Client.GetAsync("/public");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        LogEvent completed = Assert.Single(application.Sink.Events, logEvent =>
            logEvent.MessageTemplate.Text == CompletionTemplate);
        Assert.False(completed.Properties.ContainsKey("TenantId"));
        Assert.False(completed.Properties.ContainsKey("UserId"));
    }

    [Fact]
    public async Task RequestLoggingDoesNotCaptureHeadersQueryOrBodies()
    {
        const string password = "password-do-not-log";
        const string token = "token-do-not-log";
        const string email = "private.user@example.test";
        const string authorization = "jwt-do-not-log";

        await using TestLoggingApplication application = await TestLoggingApplication.StartAsync();
        using HttpRequestMessage request = new(
            HttpMethod.Post,
            $"/sensitive?email={email}&token={token}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
        request.Content = JsonContent.Create(new
        {
            Password = password,
            ConfirmPassword = password,
            Token = token
        });

        using HttpResponseMessage response = await application.Client.SendAsync(request);
        string logs = application.Sink.RenderAll();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(password, logs, StringComparison.Ordinal);
        Assert.DoesNotContain(token, logs, StringComparison.Ordinal);
        Assert.DoesNotContain(email, logs, StringComparison.Ordinal);
        Assert.DoesNotContain(authorization, logs, StringComparison.Ordinal);
        Assert.DoesNotContain("Authorization", logs, StringComparison.OrdinalIgnoreCase);
    }

    private static void AssertProperty<T>(
        LogEvent logEvent,
        string propertyName,
        T expected)
    {
        ScalarValue scalarValue = Assert.IsType<ScalarValue>(logEvent.Properties[propertyName]);
        Assert.Equal(expected, scalarValue.Value);
    }
}
