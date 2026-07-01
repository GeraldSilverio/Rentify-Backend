using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Vehicles;

public sealed class VehicleImageConfiguration : IEntityTypeConfiguration<VehicleImage>
{
    public void Configure(EntityTypeBuilder<VehicleImage> builder)
    {
        builder.ToTable("VehicleImages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.VehicleId)
            .IsRequired();

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.PublicId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.IsPrimary)
            .IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.VehicleId });
        builder.HasIndex(x => new { x.VehicleId, x.IsPrimary });
        builder.HasIndex(x => x.PublicId).IsUnique();

        builder.HasOne(x => x.Vehicle)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
