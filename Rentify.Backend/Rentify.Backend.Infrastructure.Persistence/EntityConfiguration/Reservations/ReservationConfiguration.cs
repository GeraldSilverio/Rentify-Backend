using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservationEntity = Rentify.Backend.Core.Domain.Entities.Reservations.Reservation;

namespace Rentify.Backend.Infrastructure.Persistence.EntityConfiguration.Reservations;

public sealed class ReservationConfiguration : IEntityTypeConfiguration<ReservationEntity>
{
    public void Configure(EntityTypeBuilder<ReservationEntity> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(30).IsRequired();
        builder.Property(x => x.DeliveryDateTime).HasColumnType("timestamp with time zone");
        builder.Property(x => x.ExpectedReturnDateTime).HasColumnType("timestamp with time zone");
        builder.Property(x => x.RentalType).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Channel).IsRequired();
        builder.Property(x => x.UnitRate).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.RentalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.SecurityDepositAmount).HasPrecision(18, 2).HasDefaultValue(0m).IsRequired();
        builder.Property(x => x.SecurityDepositRequired).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.DeliveryLocationName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.DeliveryAddressDetails).HasMaxLength(500);
        builder.Property(x => x.DeliveryFee).HasPrecision(18, 2).HasDefaultValue(0m).IsRequired();
        builder.Property(x => x.ReturnLocationName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.ReturnAddressDetails).HasMaxLength(500);
        builder.Property(x => x.ReturnFee).HasPrecision(18, 2).HasDefaultValue(0m).IsRequired();
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2).HasDefaultValue(0m).IsRequired();
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.ApprovedBy).HasMaxLength(150);
        builder.Property(x => x.RejectionReason).HasMaxLength(500);
        builder.Property(x => x.RejectedBy).HasMaxLength(150);
        builder.Property(x => x.CancellationReason).HasMaxLength(500);
        builder.Property(x => x.CancelledBy).HasMaxLength(150);
        builder.Property(x => x.ConvertedToRentalBy).HasMaxLength(150);
        builder.Property(x => x.CreatedBy).HasMaxLength(150).IsRequired();
        builder.Property(x => x.ModifiedBy).HasMaxLength(150).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique().HasFilter("\"IsDeleted\" = false");
        builder.HasIndex(x => new { x.TenantId, x.CustomerId });
        builder.HasIndex(x => new { x.TenantId, x.VehicleId });
        builder.HasIndex(x => new { x.TenantId, x.Status });
        builder.HasIndex(x => new { x.TenantId, x.DeliveryDateTime });
        builder.HasIndex(x => new { x.TenantId, x.ExpectedReturnDateTime });
        builder.HasIndex(x => new { x.TenantId, x.VehicleId, x.DeliveryDateTime, x.ExpectedReturnDateTime });
        builder.HasIndex(x => x.DeliveryTenantLocationId);
        builder.HasIndex(x => x.ReturnTenantLocationId);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Vehicle).WithMany().HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Restrict);
    }
}
