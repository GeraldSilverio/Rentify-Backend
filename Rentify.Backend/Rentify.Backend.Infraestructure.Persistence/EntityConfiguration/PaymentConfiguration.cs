using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Method).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Reference).HasMaxLength(100);

        builder.HasIndex(x => new { x.TenantId, x.ReservationId });
        builder.HasIndex(x => new { x.TenantId, x.PaidAt });

        builder.HasOne(x => x.Reservation)
            .WithMany()
            .HasForeignKey(x => x.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Invoice)
            .WithOne(x => x.Payment)
            .HasForeignKey<Invoice>(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
