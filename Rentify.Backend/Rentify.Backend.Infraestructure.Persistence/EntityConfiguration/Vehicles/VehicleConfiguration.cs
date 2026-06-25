using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Vehicles;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Backend.Core.Domain.Entities.Vehicles.Vehicle>
{
    public void Configure(EntityTypeBuilder<Backend.Core.Domain.Entities.Vehicles.Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlateNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => new { x.TenantId, x.PlateNumber })
            .IsUnique();

        builder.Property(x => x.Vin)
            .IsRequired()
            .HasMaxLength(17);

        builder.HasIndex(x => new { x.TenantId, x.Vin })
            .IsUnique();

        builder.Property(x => x.Color)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DailyRate)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(x => new { x.TenantId, x.Status });

        builder.HasOne(x => x.VehicleModel)
            .WithMany()
            .HasForeignKey(x => x.VehicleModelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.VehicleType)
            .WithMany()
            .HasForeignKey(x => x.VehicleTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Images)
            .WithOne(x => x.Vehicle)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Images)
            .HasField("_images")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.UnavailableDates)
            .WithOne(x => x.Vehicle)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.UnavailableDates)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
