using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Emails.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Infraestructure.Shared.Emailing;

public sealed class ResendEmailProviderSender : IEmailProviderSender
{
    private static readonly HttpClient HttpClient = new()
    {
        BaseAddress = new Uri("https://api.resend.com")
    };
    private readonly ILogger<ResendEmailProviderSender> _logger;

    public ResendEmailProviderSender(ILogger<ResendEmailProviderSender> logger)
    {
        _logger = logger;
    }

    public EmailProviderType Provider => EmailProviderType.Resend;

    public async Task<string> SendAsync(EmailProviderSendRequest request, CancellationToken cancellationToken = default)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "External provider {Provider} started operation {Operation}",
            Provider,
            "SendEmail");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/emails");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.ApiKey);
        httpRequest.Content = JsonContent.Create(new
        {
            from = $"{request.FromName} <{request.FromEmail}>",
            to = new[] { request.To },
            subject = request.Subject,
            html = request.HtmlBody,
            text = request.TextBody
        });

        using var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            stopwatch.Stop();
            _logger.LogWarning(
                "External provider {Provider} operation {Operation} failed with status {StatusCode} in {ElapsedMilliseconds} ms",
                Provider,
                "SendEmail",
                (int)response.StatusCode,
                stopwatch.ElapsedMilliseconds);

            throw new ApiException("Resend email request failed.", StatusCodes.Status400BadRequest);
        }

        var resendResponse = JsonSerializer.Deserialize<ResendEmailResponse>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        string messageId = resendResponse?.Id ?? string.Empty;
        stopwatch.Stop();

        _logger.LogInformation(
            "External provider {Provider} completed operation {Operation} with provider message {ProviderMessageId} in {ElapsedMilliseconds} ms",
            Provider,
            "SendEmail",
            messageId,
            stopwatch.ElapsedMilliseconds);

        return messageId;
    }

    private sealed record ResendEmailResponse(string Id);
}
