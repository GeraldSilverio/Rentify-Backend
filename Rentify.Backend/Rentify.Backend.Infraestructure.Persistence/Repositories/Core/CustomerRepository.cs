using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Customers;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly RentifyContext _context;

    public CustomerRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(x => x.Documents)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == customerId && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> SearchAsync(Guid tenantId, string? searchTerm, CancellationToken cancellationToken = default)
    {
        IQueryable<Customer> query = _context.Customers
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string term = searchTerm.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.FirstName.ToLower().Contains(term)
                || x.LastName.ToLower().Contains(term)
                || x.Email.ToLower().Contains(term)
                || x.PhoneNumber.ToLower().Contains(term));
        }

        return await query
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToListAsync(cancellationToken);
    }
}
