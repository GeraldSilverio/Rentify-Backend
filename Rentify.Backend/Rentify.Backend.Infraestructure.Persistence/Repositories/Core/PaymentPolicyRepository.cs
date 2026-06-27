using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Core.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Payments;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories.Core
{
    public class PaymentPolicyRepository(RentifyContext context) : IPaymentPolicyRepository
    {
        public async Task AddAsync(PaymentPolicy paymentPolicy, CancellationToken cancellation)
        {
            await context.PaymentPolicies.AddAsync(paymentPolicy, cancellation);
        }

        public async Task<PaymentPolicy?> GetDefaultByTenantIdAsync(
            Guid tenantId,
            CancellationToken cancellationToken = default)
        {
            return await context.PaymentPolicies
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IsDefault && !x.IsDeleted, cancellationToken);
        }
    }
}
