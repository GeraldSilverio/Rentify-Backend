using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Reservations;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Reservation;

public sealed class ReservationVehicleConfiguration : IEntityTypeConfiguration<ReservationVehicle>
{
    public void Configure(EntityTypeBuilder<ReservationVehicle> builder)
    {
        builder.ToTable("ReservationVehicles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DailyRate).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);

        builder.HasIndex(x => new { x.TenantId, x.VehicleId });
        builder.HasIndex(x => new { x.ReservationId, x.VehicleId }).IsUnique();

        builder.HasOne(x => x.Vehicle)
            .WithMany()
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
