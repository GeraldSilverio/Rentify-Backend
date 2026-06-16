using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration;

public sealed class VehicleBrandConfiguration : IEntityTypeConfiguration<VehicleBrand>
{
    public void Configure(EntityTypeBuilder<VehicleBrand> builder)
    {
        builder.ToTable("VehicleBrands");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasMany(x => x.Models)
            .WithOne(x => x.VehicleBrand)
            .HasForeignKey(x => x.VehicleBrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Models)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
