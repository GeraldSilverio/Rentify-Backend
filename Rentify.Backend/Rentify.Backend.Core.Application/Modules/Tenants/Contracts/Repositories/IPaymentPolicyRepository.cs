using Rentify.Backend.Core.Domain.Entities.Payments;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories
{
    public interface IPaymentPolicyRepository
    {
        Task AddAsync(PaymentPolicy paymentPolicy,CancellationToken cancellation);

        Task<PaymentPolicy?> GetDefaultByTenantIdAsync(
            Guid tenantId,
            CancellationToken cancellationToken = default);
    }
}
