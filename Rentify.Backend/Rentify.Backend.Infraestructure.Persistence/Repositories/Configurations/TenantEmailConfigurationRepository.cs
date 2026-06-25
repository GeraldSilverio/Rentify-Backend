using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infrastructure.Persistence.Repositories
{
    public sealed class TenantEmailConfigurationRepository : ITenantEmailConfigurationRepository
    {
        private readonly RentifyContext _context;

        public TenantEmailConfigurationRepository(RentifyContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TenantEmailConfiguration tenantEmailConfiguration, CancellationToken cancellationToken = default)
        {
            await _context.TenantEmailConfigurations.AddAsync(tenantEmailConfiguration, cancellationToken);
        }

        public async Task<IReadOnlyList<TenantEmailConfiguration>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantEmailConfigurations
                .Where(x => x.TenantId == tenantId && !x.IsDeleted)
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.Provider)
                .ToListAsync(cancellationToken);
        }

        public async Task<TenantEmailConfiguration?> GetByProviderAsync(Guid tenantId, EmailProviderType provider, CancellationToken cancellationToken = default)
        {
            return await _context.TenantEmailConfigurations
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Provider == provider && !x.IsDeleted, cancellationToken);
        }

        public async Task<TenantEmailConfiguration?> GetDefaultAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantEmailConfigurations
                .Where(x => x.TenantId == tenantId && x.IsActive && !x.IsDeleted)
                .OrderByDescending(x => x.IsDefault)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
