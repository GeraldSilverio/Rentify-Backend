using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Emails.Dtos;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Infraestructure.Persistence.Emailing
{
    public class ResendEmailProviderSender : IEmailProviderSender
    {
        private static readonly HttpClient HttpClient = new()
        {
            BaseAddress = new Uri("https://api.resend.com")
        };

        public EmailProviderType Provider => EmailProviderType.Resend;

        public async Task<string> SendAsync(EmailProviderSendRequest request, CancellationToken cancellationToken = default)
        {
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
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"Resend email failed: {responseContent}", StatusCodes.Status400BadRequest);
            }

            var resendResponse = JsonSerializer.Deserialize<ResendEmailResponse>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return resendResponse?.Id ?? string.Empty;
        }

        private sealed record ResendEmailResponse(string Id);
    }
}
