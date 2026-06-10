using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infrastructure.Persistence.Repositories
{
    public sealed class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly RentifyContext _context;

        public EmailTemplateRepository(RentifyContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmailTemplate emailTemplate, CancellationToken cancellationToken = default)
        {
            await _context.EmailTemplates.AddAsync(emailTemplate, cancellationToken);
        }

        public async Task<IReadOnlyList<EmailTemplate>> ListAsync(Guid? tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.EmailTemplates
                .Where(x => !x.IsDeleted)
                .Where(x => tenantId == null || x.TenantId == tenantId || x.TenantId == null)
                .OrderBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task<EmailTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.EmailTemplates
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<EmailTemplate?> GetByCodeAsync(Guid? tenantId, string code, CancellationToken cancellationToken = default)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return await _context.EmailTemplates
                .Where(x => x.Code == normalizedCode && x.IsActive && !x.IsDeleted)
                .Where(x => x.TenantId == tenantId || x.TenantId == null)
                .OrderByDescending(x => x.TenantId == tenantId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(Guid? tenantId, string code, CancellationToken cancellationToken = default)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return await _context.EmailTemplates
                .AnyAsync(x => x.TenantId == tenantId && x.Code == normalizedCode && !x.IsDeleted, cancellationToken);
        }
    }
}
