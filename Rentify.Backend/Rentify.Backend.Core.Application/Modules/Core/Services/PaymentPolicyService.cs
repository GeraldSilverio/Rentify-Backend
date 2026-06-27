using Rentify.Backend.Core.Application.Modules.Core.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Core.Contracts.Services;
using Rentify.Backend.Core.Domain.Entities.Payments;

namespace Rentify.Backend.Core.Application.Modules.Core.Services
{
    public class PaymentPolicyService(IPaymentPolicyRepository paymentPolicyRepository) : IPaymentPolicyService
    {
        public async Task AddAsync(PaymentPolicy paymentPolicy,CancellationToken cancellationToken)
        {
            await paymentPolicyRepository.AddAsync(paymentPolicy, cancellationToken);
        }
    }
}
