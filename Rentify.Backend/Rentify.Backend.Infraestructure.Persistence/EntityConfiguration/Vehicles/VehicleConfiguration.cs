using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Vehicles;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Backend.Core.Domain.Entities.Vehicles.Vehicle>
{
    public void Configure(EntityTypeBuilder<Backend.Core.Domain.Entities.Vehicles.Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.VehicleBrandId)
            .IsRequired();

        builder.Property(x => x.VehicleModelId)
            .IsRequired();

        builder.Property(x => x.VehicleTypeId)
            .IsRequired();

        builder.Property(x => x.PlateNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => new { x.TenantId, x.PlateNumber })
            .IsUnique();

        builder.Property(x => x.Vin)
            .HasMaxLength(50);

        builder.HasIndex(x => new { x.TenantId, x.Vin })
            .IsUnique();

        builder.Property(x => x.Color)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(x => new { x.TenantId, x.Status });

        builder.HasIndex(x => new { x.TenantId, x.VehicleBrandId });
        builder.HasIndex(x => new { x.TenantId, x.VehicleModelId });
        builder.HasIndex(x => new { x.TenantId, x.VehicleTypeId });

        builder.Ignore(x => x.DailyRate);

        builder.HasOne(x => x.VehicleBrand)
            .WithMany()
            .HasForeignKey(x => x.VehicleBrandId)
            .OnDelete(DeleteBehavior.Restrict);

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

        builder.HasMany(x => x.Rates)
            .WithOne(x => x.Vehicle)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Rates)
            .HasField("_rates")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.UnavailableDates)
            .WithOne(x => x.Vehicle)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.UnavailableDates)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
