using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories
{
    public interface IEmailTemplateRepository
    {
        Task AddAsync(EmailTemplate emailTemplate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<EmailTemplate>> ListAsync(Guid? tenantId, CancellationToken cancellationToken = default);
        Task<EmailTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<EmailTemplate?> GetByCodeAsync(Guid? tenantId, string code, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(Guid? tenantId, string code, CancellationToken cancellationToken = default);
    }
}
