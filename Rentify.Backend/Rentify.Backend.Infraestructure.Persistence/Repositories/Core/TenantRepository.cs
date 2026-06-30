using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infrastructure.Persistence.Repositories;

public sealed class TenantRepository : ITenantRepository
{
    private readonly RentifyContext _context;

    public TenantRepository(RentifyContext context)
    {
        _context = context;
    }
    public async Task AddAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default)
    {
        await _context.Tenants.AddAsync(
            tenant,
            cancellationToken);
    }

    public async Task<Tenant?> GetByIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(x => x.Id == tenantId && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> RncExistsForAnotherTenantAsync(
        Guid tenantId,
        string normalizedRnc,
        CancellationToken cancellationToken = default)
    {
        return await RncExistsAsync(normalizedRnc, tenantId, cancellationToken);
    }

    public async Task<bool> RncExistsAsync(
        string normalizedRnc,
        Guid? excludedTenantId = null,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AsNoTracking()
            .AnyAsync(
                x => (!excludedTenantId.HasValue || x.Id != excludedTenantId.Value)
                     && !x.IsDeleted
                     && x.Rnc == normalizedRnc,
                cancellationToken);
    }
}
