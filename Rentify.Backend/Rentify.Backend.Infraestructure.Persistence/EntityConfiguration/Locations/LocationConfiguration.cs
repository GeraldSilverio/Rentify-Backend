using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Locations;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Locations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.Province)
            .HasMaxLength(100);

        builder.Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Address)
            .HasMaxLength(250);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.ModifiedBy).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.ModifiedDate).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasIndex(x => new { x.Name, x.City, x.Province, x.Country })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => x.IsActive);
    }
}
