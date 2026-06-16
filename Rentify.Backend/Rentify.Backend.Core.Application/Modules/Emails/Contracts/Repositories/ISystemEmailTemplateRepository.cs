using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories
{
    public interface ISystemEmailTemplateRepository
    {
        Task AddAsync(SystemEmailTemplate emailTemplate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SystemEmailTemplate>> ListAsync(Guid? tenantId, CancellationToken cancellationToken = default);
        Task<SystemEmailTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<SystemEmailTemplate?> GetByCodeAsync(Guid? tenantId, string code, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(Guid? tenantId, string code, CancellationToken cancellationToken = default);
    }
}
