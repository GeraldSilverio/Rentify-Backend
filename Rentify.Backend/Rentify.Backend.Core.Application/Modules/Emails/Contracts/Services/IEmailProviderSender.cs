using Rentify.Backend.Core.Application.Modules.Emails.Dtos;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services
{
    public interface IEmailProviderSender
    {
        EmailProviderType Provider { get; }
        Task<string> SendAsync(EmailProviderSendRequest request, CancellationToken cancellationToken = default);
    }
}
