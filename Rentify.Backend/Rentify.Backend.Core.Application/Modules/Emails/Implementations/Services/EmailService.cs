using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Emails.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Helpers;
using Rentify.Backend.Core.Domain.Enums;
using System.Text.Encodings.Web;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Rentify.Backend.Core.Application.Modules.Emails.Implementations.Services
{
    public class EmailService : IEmailService
    {
        private static readonly Regex UnresolvedPlaceholderPattern = new(
            @"\{\{\s*[^{}]+\s*\}\}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly ISystemEmailTemplateRepository _emailTemplateRepository;
        private readonly IEnumerable<IEmailProviderSender> _emailProviderSenders;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            ISystemEmailTemplateRepository emailTemplateRepository,
            IEnumerable<IEmailProviderSender> emailProviderSenders,
            ILogger<EmailService> logger)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _emailProviderSenders = emailProviderSenders;
            _logger = logger;
        }

        public async Task<SendTemplateEmailResponse> SendEmailAsync(SendTemplateEmailCommand command, CancellationToken cancellationToken = default)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "Sending email using template {EmailTemplateCode} for Tenant {TenantId}",
                command.TemplateCode,
                command.TenantId);

            var emailTemplate = await _emailTemplateRepository.GetByCodeAsync(command.TenantId, command.TemplateCode, cancellationToken);

            if (emailTemplate == null)
            {
                _logger.LogError(
                    "Active email template {EmailTemplateCode} was not found for Tenant {TenantId}",
                    command.TemplateCode,
                    command.TenantId);
                throw new ApiException("Email template not found", StatusCodes.Status404NotFound);
            }

            var emailProviderSender = _emailProviderSenders.FirstOrDefault(x => x.Provider == EmailProviderType.Resend);

            if (emailProviderSender == null)
            {
                throw new ApiException($"Email provider {EmailProviderType.Resend} is not implemented", StatusCodes.Status400BadRequest);
            }

            var variables = command.Variables ?? new Dictionary<string, string>();
            var subject = RenderTemplate(emailTemplate.Subject, variables);
            var htmlBody = RenderHtmlTemplate(emailTemplate.HtmlBody, variables);
            var textBody = emailTemplate.TextBody == null ? null : RenderTemplate(emailTemplate.TextBody, variables);

            var messageId = await emailProviderSender.SendAsync(
                new EmailProviderSendRequest(
                    ReadFromConfiguration.GetValueFromConfig("RESEND_API_KEY"),
                    ReadFromConfiguration.GetValueFromConfig("EMAIL_FROM"),
                    ReadFromConfiguration.GetValueFromConfig("EMAIL_FROM_NAME"),
                    command.To,
                    subject,
                    htmlBody,
                    textBody),
                cancellationToken);

            stopwatch.Stop();

            _logger.LogInformation(
                "Email sent using template {EmailTemplateCode} for Tenant {TenantId} with provider message {ProviderMessageId} in {ElapsedMilliseconds} ms",
                command.TemplateCode,
                command.TenantId,
                messageId,
                stopwatch.ElapsedMilliseconds);

            return new SendTemplateEmailResponse(EmailProviderType.Resend.ToString(), messageId);
        }

        private static string RenderTemplate(string template, Dictionary<string, string> variables)
        {
            foreach (var variable in variables)
            {
                template = template
                    .Replace($"{{{{{variable.Key}}}}}", variable.Value)
                    .Replace($"{{{{ {variable.Key} }}}}", variable.Value);
            }

            return template;
        }

        private static string RenderHtmlTemplate(string template, Dictionary<string, string> variables)
        {
            Dictionary<string, string> encodedVariables = variables.ToDictionary(
                variable => variable.Key,
                variable => HtmlEncoder.Default.Encode(variable.Value));

            return RenderTemplate(template, encodedVariables);
        }

    }
}
