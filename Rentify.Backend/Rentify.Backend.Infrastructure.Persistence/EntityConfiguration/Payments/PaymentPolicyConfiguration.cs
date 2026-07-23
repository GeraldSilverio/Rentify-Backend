using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Payments;

public sealed class PaymentPolicyConfiguration : IEntityTypeConfiguration<PaymentPolicy>
{
    public void Configure(EntityTypeBuilder<PaymentPolicy> builder)
    {
        builder.ToTable("PaymentPolicies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PaymentFrequency)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CutoffDayOfWeek)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.GraceDays)
            .IsRequired();

        builder.Property(x => x.ReminderStartDayOfWeek)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.LateFeeEnabled)
            .IsRequired();

        builder.Property(x => x.IsDefault)
            .IsRequired();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TenantId);

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.IsDefault
        });
    }
}