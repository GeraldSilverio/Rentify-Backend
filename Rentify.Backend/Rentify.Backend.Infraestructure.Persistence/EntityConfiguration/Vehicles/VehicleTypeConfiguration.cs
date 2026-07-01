using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Vehicles;

public sealed class VehicleTypeConfiguration : IEntityTypeConfiguration<VehicleType>
{
    public void Configure(EntityTypeBuilder<VehicleType> builder)
    {
        builder.ToTable("VehicleTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
