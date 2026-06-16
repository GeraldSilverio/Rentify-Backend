using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration;

public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.PaymentStatus).HasConversion<string>().HasMaxLength(40);

        builder.HasIndex(x => new { x.TenantId, x.CustomerId });
        builder.HasIndex(x => new { x.TenantId, x.Status });
        builder.HasIndex(x => new { x.TenantId, x.StartDate, x.EndDate });

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Vehicles)
            .WithOne(x => x.Reservation)
            .HasForeignKey(x => x.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Vehicles)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Payments)
            .WithOne(x => x.Reservation)
            .HasForeignKey(x => x.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Payments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
