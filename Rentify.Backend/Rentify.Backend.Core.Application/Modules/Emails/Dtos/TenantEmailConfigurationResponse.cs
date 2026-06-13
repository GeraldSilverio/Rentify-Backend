using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Emails.Dtos
{
    public record TenantEmailConfigurationResponse(
        Guid Id,
        Guid TenantId,
        EmailProviderType Provider,
        string FromEmail,
        string FromName,
        bool IsDefault,
        bool IsActive,
        string ApiKeyPreview);
}
