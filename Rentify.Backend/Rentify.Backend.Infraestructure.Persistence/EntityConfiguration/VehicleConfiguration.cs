using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Make)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PlateNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.PlateNumber)
            .IsUnique();

        builder.Property(x => x.Vin)
            .IsRequired()
            .HasMaxLength(17);

        builder.HasIndex(x => x.Vin)
            .IsUnique();

        builder.Property(x => x.Color)
            .HasMaxLength(50);

        builder.Property(x => x.DailyRate)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(x => x.RentCar)
            .WithMany()
            .HasForeignKey(x => x.RentCarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Images)
            .WithOne(x => x.Vehicle)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.UnavailableDates)
            .WithOne(x => x.Vehicle)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Images)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.UnavailableDates)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
