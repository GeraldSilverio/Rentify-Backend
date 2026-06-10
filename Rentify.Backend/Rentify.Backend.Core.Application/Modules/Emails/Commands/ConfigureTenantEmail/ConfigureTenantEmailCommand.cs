using MediatR;
using Rentify.Backend.Core.Application.Shared.Response;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Emails.Commands.ConfigureTenantEmail
{
    public record ConfigureTenantEmailCommand(
        Guid TenantId,
        EmailProviderType Provider,
        string ApiKey,
        string FromEmail,
        string FromName,
        bool IsDefault,
        string CreatedBy) : IRequest<ResultReponse<Guid>>;
}
