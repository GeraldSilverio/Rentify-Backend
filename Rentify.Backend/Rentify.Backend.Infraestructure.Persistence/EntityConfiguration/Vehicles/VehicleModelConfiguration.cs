using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Vehicles;

public sealed class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
{
    public void Configure(EntityTypeBuilder<VehicleModel> builder)
    {
        builder.ToTable("VehicleModels");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => new { x.VehicleBrandId, x.Name })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
