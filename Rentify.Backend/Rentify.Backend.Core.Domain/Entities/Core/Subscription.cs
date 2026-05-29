using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Core
{
    public class Subscription : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid TenantId { get; private set; }

        public Guid SubscriptionPlanId { get; private set; }

        public SubscriptionStatus Status { get; private set; }

        public DateTime StartsAt { get; private set; }

        public DateTime ExpiresAt { get; private set; }

        public bool AutoRenew { get; private set; }

        public DateTime? CancelledAt { get; private set; }

        public Tenant Tenant { get; private set; } = null!;

        public SubscriptionPlan SubscriptionPlan { get; private set; } = null!;

        // EF
        private Subscription()
        {
        }

        private Subscription(
            Guid tenantId,
            Guid subscriptionPlanId,
            DateTime startsAt,
            DateTime expiresAt,
            string createdBy)
        {
            Id = Guid.NewGuid();

            TenantId = tenantId;
            SubscriptionPlanId = subscriptionPlanId;

            StartsAt = startsAt;
            ExpiresAt = expiresAt;

            Status = SubscriptionStatus.Active;
            AutoRenew = true;

            CreatedDate = DateTime.UtcNow;
            CreatedBy = createdBy;
            ModifiedBy = createdBy;
            ModifiedDate = CreatedDate;
        }

        public static Subscription Create(
            Guid tenantId,
            Guid subscriptionPlanId,
            int durationInDays,
            string createdBy)
        {
            var now = DateTime.UtcNow;

            return new Subscription(
                tenantId,
                subscriptionPlanId,
                now,
                now.AddDays(durationInDays),
                createdBy);
        }

        public void Cancel(string modifiedBy)
        {
            Status = SubscriptionStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;

            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }

        public void Renew(int durationInDays, string modifiedBy)
        {
            ExpiresAt = ExpiresAt.AddDays(durationInDays);

            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiresAt;
        }
    }
}
