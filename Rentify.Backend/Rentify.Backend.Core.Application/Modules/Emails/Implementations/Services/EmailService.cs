using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Emails.Dtos;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Helpers;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Emails.Implementations.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISystemEmailTemplateRepository _emailTemplateRepository;
        private readonly IEnumerable<IEmailProviderSender> _emailProviderSenders;

        public EmailService(
            ISystemEmailTemplateRepository emailTemplateRepository,
            IEnumerable<IEmailProviderSender> emailProviderSenders)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _emailProviderSenders = emailProviderSenders;
        }

        public async Task<SendTemplateEmailResponse> SendEmailAsync(SendTemplateEmailCommand command, CancellationToken cancellationToken = default)
        {
            var emailTemplate = await _emailTemplateRepository.GetByCodeAsync(command.TenantId, command.TemplateCode, cancellationToken);

            if (emailTemplate == null)
            {
                throw new ApiException("Email template not found", StatusCodes.Status404NotFound);
            }

            var emailProviderSender = _emailProviderSenders.FirstOrDefault(x => x.Provider == EmailProviderType.Resend);

            if (emailProviderSender == null)
            {
                throw new ApiException($"Email provider {EmailProviderType.Resend} is not implemented", StatusCodes.Status400BadRequest);
            }

            var variables = command.Variables ?? new Dictionary<string, string>();
            var subject = RenderTemplate(emailTemplate.Subject, variables);
            var htmlBody = RenderTemplate(emailTemplate.HtmlBody, variables);
            var textBody = emailTemplate.TextBody == null ? null : RenderTemplate(emailTemplate.TextBody, variables);

            var messageId = await emailProviderSender.SendAsync(
                new EmailProviderSendRequest(
                    ReadFromConfiguration.GetValueFromConfig("RESEND_API_KEY"),
                    ReadFromConfiguration.GetValueFromConfig("EMAIL_FROM"),
                    ReadFromConfiguration.GetValueFromConfig("EMAIL_FROM_NAME"),
                    variables.TryGetValue("OwnerEmail", out var ownerEmail) ? ownerEmail : "",
                    subject,
                    htmlBody,
                    textBody),
                cancellationToken);

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
    }
}
