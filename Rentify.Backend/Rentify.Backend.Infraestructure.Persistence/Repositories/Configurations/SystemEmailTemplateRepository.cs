using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories
{
    public sealed class SystemEmailTemplateRepository : ISystemEmailTemplateRepository
    {
        private readonly RentifyContext _context;

        public SystemEmailTemplateRepository(RentifyContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SystemEmailTemplate emailTemplate, CancellationToken cancellationToken = default)
        {
            await _context.SystemEmailTemplates.AddAsync(emailTemplate, cancellationToken);
        }

        public async Task<IReadOnlyList<SystemEmailTemplate>> ListAsync(Guid? tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.SystemEmailTemplates
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task<SystemEmailTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.SystemEmailTemplates
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<SystemEmailTemplate?> GetByCodeAsync(Guid? tenantId, string code, CancellationToken cancellationToken = default)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return await _context.SystemEmailTemplates
                .Where(x => x.Code == normalizedCode && x.IsActive && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(Guid? tenantId, string code, CancellationToken cancellationToken = default)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return await _context.SystemEmailTemplates
                .AnyAsync(x => x.Code == normalizedCode && !x.IsDeleted, cancellationToken);
        }
    }
}
