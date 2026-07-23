using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Modules.Customers.Queries.SearchCustomers;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Customers;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infrastructure.Persistence.Context;

namespace Rentify.Backend.Infrastructure.Persistence.Repositories;

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

    public async Task AddDocumentAsync(CustomerDocument document, CancellationToken cancellationToken = default)
    {
        await _context.CustomerDocuments.AddAsync(document, cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(x => x.Documents)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == customerId && !x.IsDeleted, cancellationToken);
    }

    public async Task<CustomerDocument?> GetDocumentByIdAsync(
        Guid tenantId,
        Guid customerId,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.CustomerDocuments
            .FirstOrDefaultAsync(document =>
                document.TenantId == tenantId
                && document.CustomerId == customerId
                && document.Id == documentId
                && !document.IsDeleted,
                cancellationToken);
    }

    public Task<bool> IdentificationExistsAsync(
        Guid tenantId,
        IdentificationType identificationType,
        string identificationNumberNormalized,
        Guid? excludedCustomerId,
        CancellationToken cancellationToken = default)
    {
        return _context.Customers.AnyAsync(customer =>
            customer.TenantId == tenantId
            && customer.IdentificationType == identificationType
            && customer.IdentificationNumberNormalized == identificationNumberNormalized
            && !customer.IsDeleted
            && (!excludedCustomerId.HasValue || customer.Id != excludedCustomerId.Value),
            cancellationToken);
    }

    public async Task<CustomerDetailsResponse?> GetDetailsAsync(
        Guid tenantId,
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id == customerId && !x.IsDeleted)
            .Select(x => new CustomerDetailsResponse(
                x.Id,
                x.TenantId,
                x.CustomerType,
                x.IdentificationType,
                x.IdentificationNumber,
                x.BirthDate,
                x.IsVerified,
                x.VerifiedAt,
                x.VerifiedBy,
                x.FirstName,
                x.LastName,
                x.Email,
                x.PhoneNumber,
                x.AddressLine,
                x.Sector,
                x.City,
                x.Province,
                x.AddressReference,
                x.IsActive,
                x.CreatedDate,
                x.ModifiedDate,
                x.Documents
                    .Where(document => document.IsActive && !document.IsDeleted)
                    .OrderBy(document => document.CreatedDate)
                    .Select(document => new CustomerDocumentResponse(
                        document.Id,
                        document.Name,
                        document.Url,
                        document.PublicId,
                        document.DocumentType,
                        document.DocumentSide,
                        document.CreatedDate))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PaginatedResponse<CustomerResponse>> SearchAsync(
        SearchCustomersQuery query,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Customer> customersQuery = _context.Customers
            .AsNoTracking()
            .Where(x => x.TenantId == query.TenantId && !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            string searchPattern = $"%{query.SearchTerm.Trim()}%";
            customersQuery = customersQuery.Where(customer =>
                EF.Functions.ILike(customer.FirstName, searchPattern)
                || EF.Functions.ILike(customer.LastName, searchPattern)
                || EF.Functions.ILike(customer.Email, searchPattern)
                || EF.Functions.ILike(customer.PhoneNumber, searchPattern));
        }

        int totalCount = await customersQuery.CountAsync(cancellationToken);
        int totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)query.PageSize);

        List<CustomerResponse> customers = await customersQuery
            .OrderBy(customer => customer.FirstName)
            .ThenBy(customer => customer.LastName)
            .ThenBy(customer => customer.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(customer => new CustomerResponse(
                customer.Id,
                customer.TenantId,
                customer.CustomerType,
                customer.IdentificationType,
                customer.IdentificationNumber,
                customer.BirthDate,
                customer.IsVerified,
                customer.FirstName,
                customer.LastName,
                customer.Email,
                customer.PhoneNumber,
                customer.AddressLine,
                customer.Sector,
                customer.City,
                customer.Province,
                customer.AddressReference))
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<CustomerResponse>(
            customers,
            query.PageNumber,
            query.PageSize,
            totalCount,
            totalPages);
    }

    public Task<bool> EmailExistsAsync(Guid tenantId, string email, CancellationToken cancellationToken = default)
    {
        return EmailExistsAsync(tenantId, email, null, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(
        Guid tenantId,
        string email,
        Guid? excludedCustomerId,
        CancellationToken cancellationToken = default)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        return _context.Customers.AnyAsync(customer =>
             customer.TenantId == tenantId
             && customer.Email == normalizedEmail
             && !customer.IsDeleted
             && (!excludedCustomerId.HasValue || customer.Id != excludedCustomerId.Value),
             cancellationToken);
    }

    public Task<bool> ExistCustomerByIdAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default)
    {
        return _context.Customers.AnyAsync(
            customer =>
                customer.TenantId == tenantId
                && customer.Id == customerId
                && !customer.IsDeleted,
            cancellationToken);
    }
}
