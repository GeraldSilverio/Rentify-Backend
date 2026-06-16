using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.ConfigureTenantEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.CreateEmailTemplate;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.UpdateEmailTemplate;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Emails.Dtos;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Application.Shared.Helpers;
using Rentify.Backend.Core.Application.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Emails.Implementations.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISystemEmailTemplateRepository _emailTemplateRepository;
        private readonly ITenantEmailConfigurationRepository _tenantEmailConfigurationRepository;
        private readonly IEnumerable<IEmailProviderSender> _emailProviderSenders;
        private readonly IUnitOfWork _unitOfWork;

        public EmailService(
            ISystemEmailTemplateRepository emailTemplateRepository,
            ITenantEmailConfigurationRepository tenantEmailConfigurationRepository,
            IEnumerable<IEmailProviderSender> emailProviderSenders,
            IUnitOfWork unitOfWork)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _tenantEmailConfigurationRepository = tenantEmailConfigurationRepository;
            _emailProviderSenders = emailProviderSenders;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateEmailTemplateAsync(CreateEmailTemplateCommand command, CancellationToken cancellationToken = default)
        {
            if (await _emailTemplateRepository.ExistsByCodeAsync(command.TenantId, command.Code, cancellationToken))
            {
                throw new ApiException("Email template code already exists", StatusCodes.Status400BadRequest);
            }

            var emailTemplate = SystemEmailTemplate.Create(
                command.Code,
                command.Name,
                command.Subject,
                command.HtmlBody,
                command.TextBody,
                command.CreatedBy);

            await _emailTemplateRepository.AddAsync(emailTemplate, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return emailTemplate.Id;
        }

        public async Task<Guid> UpdateEmailTemplateAsync(Guid id, UpdateEmailTemplateCommand command, CancellationToken cancellationToken = default)
        {
            var emailTemplate = await _emailTemplateRepository.GetByIdAsync(id, cancellationToken);

            if (emailTemplate == null)
            {
                throw new ApiException("Email template not found", StatusCodes.Status404NotFound);
            }

            emailTemplate.Update(
                command.Name,
                command.Subject,
                command.HtmlBody,
                command.TextBody,
                command.ModifiedBy);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return emailTemplate.Id;
        }

        public async Task<Guid> ConfigureTenantEmailAsync(ConfigureTenantEmailCommand command, CancellationToken cancellationToken = default)
        {
            var tenantEmailConfiguration = await _tenantEmailConfigurationRepository.GetByProviderAsync(
                command.TenantId,
                command.Provider,
                cancellationToken);

            if (tenantEmailConfiguration == null)
            {
                tenantEmailConfiguration = TenantEmailConfiguration.Create(
                    command.TenantId,
                    command.Provider,
                    command.ApiKey,
                    command.FromEmail,
                    command.FromName,
                    command.IsDefault,
                    command.CreatedBy);

                await _tenantEmailConfigurationRepository.AddAsync(tenantEmailConfiguration, cancellationToken);
            }
            else
            {
                tenantEmailConfiguration.Update(
                    command.ApiKey,
                    command.FromEmail,
                    command.FromName,
                    command.IsDefault,
                    command.CreatedBy);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return tenantEmailConfiguration.Id;
        }

        public async Task<SendTemplateEmailResponse> SendEmailAsync(SendTemplateEmailCommand command, CancellationToken cancellationToken = default)
        {
            //var tenantEmailConfiguration = await _tenantEmailConfigurationRepository.GetDefaultAsync(command.TenantId, cancellationToken);

            //if (tenantEmailConfiguration == null)
            //{
            //    throw new ApiException("Tenant email configuration not found", StatusCodes.Status404NotFound);
            //}

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
                    command.To,
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
