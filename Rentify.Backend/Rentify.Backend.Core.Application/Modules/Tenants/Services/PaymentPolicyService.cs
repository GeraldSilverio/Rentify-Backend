using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Domain.Entities.Payments;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Services
{
    public class PaymentPolicyService(IPaymentPolicyRepository paymentPolicyRepository) : IPaymentPolicyService
    {
        public async Task AddAsync(PaymentPolicy paymentPolicy,CancellationToken cancellationToken)
        {
            await paymentPolicyRepository.AddAsync(paymentPolicy, cancellationToken);
        }
    }
}
