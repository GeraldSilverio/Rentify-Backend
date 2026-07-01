using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Vehicles;

public sealed class VehicleFeatureAssignmentConfiguration : IEntityTypeConfiguration<VehicleFeatureAssignment>
{
    public void Configure(EntityTypeBuilder<VehicleFeatureAssignment> builder)
    {
        builder.ToTable("VehicleFeatureAssignments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.VehicleId)
            .IsRequired();

        builder.Property(x => x.VehicleFeatureId)
            .IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.VehicleId, x.VehicleFeatureId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasOne(x => x.Vehicle)
            .WithMany(x => x.FeatureAssignments)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.VehicleFeature)
            .WithMany()
            .HasForeignKey(x => x.VehicleFeatureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
