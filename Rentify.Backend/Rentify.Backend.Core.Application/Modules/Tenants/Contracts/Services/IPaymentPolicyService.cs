using Rentify.Backend.Core.Domain.Entities.Payments;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services
{
    public interface IPaymentPolicyService
    {
        Task AddAsync(PaymentPolicy paymentPolicy, CancellationToken cancellationToken);
    }
}
