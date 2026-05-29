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

    public async Task<bool> SlugExistsAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AnyAsync(
                x => x.Slug == slug,
                cancellationToken);
    }

    public async Task AddAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default)
    {
        await _context.Tenants.AddAsync(
            tenant,
            cancellationToken);
    }
}