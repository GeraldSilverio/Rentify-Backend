using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Payments.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly RentifyContext _context;

    public PaymentRepository(RentifyContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
    }

    public async Task AddInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        await _context.Invoices.AddAsync(invoice, cancellationToken);
    }

    public async Task<string> GenerateInvoiceNumberAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        string prefix = $"INV-{DateTime.UtcNow:yyyyMM}";
        int count = await _context.Invoices.CountAsync(
            x => x.TenantId == tenantId && x.InvoiceNumber.StartsWith(prefix),
            cancellationToken);

        return $"{prefix}-{count + 1:D5}";
    }
}
