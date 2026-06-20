using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Payments;

namespace Rentify.Backend.Core.Application.Modules.Payments.Contracts.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    Task AddInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<string> GenerateInvoiceNumberAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
