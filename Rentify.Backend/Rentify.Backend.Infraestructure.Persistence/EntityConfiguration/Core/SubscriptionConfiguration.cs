using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Core;

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.StartsAt)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.TrialEndsAt);

        builder.Property(x => x.IsTrial)
            .IsRequired();

        builder.Property(x => x.AutoRenew)
            .IsRequired();

        builder.Property(x => x.CancelledAt);

        // Audit
        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        builder.Property(x => x.ModifiedDate)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SubscriptionPlan)
            .WithMany()
            .HasForeignKey(x => x.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TenantId);

        builder.HasIndex(x => x.SubscriptionPlanId);

        builder.HasIndex(x => x.Status);
    }
}
