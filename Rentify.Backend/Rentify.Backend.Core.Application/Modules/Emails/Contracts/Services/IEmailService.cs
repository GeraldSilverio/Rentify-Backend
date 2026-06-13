using Rentify.Backend.Core.Application.Modules.Emails.Commands.ConfigureTenantEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.CreateEmailTemplate;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.UpdateEmailTemplate;

namespace Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services
{
    public interface IEmailService
    {
        Task<Guid> CreateEmailTemplateAsync(CreateEmailTemplateCommand command, CancellationToken cancellationToken = default);
        Task<Guid> UpdateEmailTemplateAsync(Guid id, UpdateEmailTemplateCommand command, CancellationToken cancellationToken = default);
        Task<Guid> ConfigureTenantEmailAsync(ConfigureTenantEmailCommand command, CancellationToken cancellationToken = default);
        Task<SendTemplateEmailResponse> SendTemplateEmailAsync(SendTemplateEmailCommand command, CancellationToken cancellationToken = default);
    }
}
