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

        public DateTime? TrialEndsAt { get; private set; }

        public bool IsTrial { get; private set; }

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
            DateTime? trialEndsAt,
            bool isTrial,
            string createdBy)
        {
            Id = Guid.NewGuid();

            TenantId = tenantId;
            SubscriptionPlanId = subscriptionPlanId;

            StartsAt = startsAt;
            ExpiresAt = expiresAt;
            TrialEndsAt = trialEndsAt;
            IsTrial = isTrial;

            Status = isTrial ? SubscriptionStatus.Trialing : SubscriptionStatus.Active;
            AutoRenew = true;
            IsActive = true;

            CreatedDate = DateTime.UtcNow;
            CreatedBy = createdBy;
            ModifiedBy = createdBy;
            ModifiedDate = CreatedDate;
        }

        public static Subscription Create(
            Guid tenantId,
            Guid subscriptionPlanId,
            int durationInDays,
            int trialDays,
            string createdBy)
        {
            var now = DateTime.UtcNow;
            var trialEndsAt = trialDays > 0 ? now.AddDays(trialDays) : (DateTime?)null;
            var expiresAt = trialEndsAt.HasValue
                ? trialEndsAt.Value.AddDays(durationInDays)
                : now.AddDays(durationInDays);

            return new Subscription(
                tenantId,
                subscriptionPlanId,
                now,
                expiresAt,
                trialEndsAt,
                trialDays > 0,
                createdBy);
        }

        public void Cancel(string modifiedBy)
        {
            Status = SubscriptionStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            IsActive = false;

            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }

        public void Renew(int durationInDays, string modifiedBy)
        {
            ExpiresAt = ExpiresAt.AddDays(durationInDays);
            IsTrial = false;
            TrialEndsAt = null;
            Status = SubscriptionStatus.Active;
            IsActive = true;

            ModifiedBy = modifiedBy;
            ModifiedDate = DateTime.UtcNow;
        }

        public void ValidateStatus(string modifiedBy)
        {
            var now = DateTime.UtcNow;

            if (now > ExpiresAt && Status != SubscriptionStatus.Expired)
            {
                Status = SubscriptionStatus.Expired;
                IsActive = false;
                ModifiedBy = modifiedBy;
                ModifiedDate = now;
                return;
            }

            if (IsTrial && TrialEndsAt.HasValue && now > TrialEndsAt.Value && now <= ExpiresAt)
            {
                IsTrial = false;
                Status = SubscriptionStatus.Active;
                ModifiedBy = modifiedBy;
                ModifiedDate = now;
            }
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiresAt;
        }
    }
}
