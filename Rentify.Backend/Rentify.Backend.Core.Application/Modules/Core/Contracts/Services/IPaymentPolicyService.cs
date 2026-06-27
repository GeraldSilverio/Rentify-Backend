using Rentify.Backend.Core.Domain.Entities.Payments;

namespace Rentify.Backend.Core.Application.Modules.Core.Contracts.Services
{
    public interface IPaymentPolicyService
    {
        Task AddAsync(PaymentPolicy paymentPolicy, CancellationToken cancellationToken);
    }
}
