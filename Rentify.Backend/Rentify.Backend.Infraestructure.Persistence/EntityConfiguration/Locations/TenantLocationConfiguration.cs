using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Locations;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Locations;

public sealed class TenantLocationConfiguration : IEntityTypeConfiguration<TenantLocation>
{
    public void Configure(EntityTypeBuilder<TenantLocation> builder)
    {
        builder.ToTable("TenantLocations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.LocationId);

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.AllowsDelivery).IsRequired();
        builder.Property(x => x.AllowsPickup).IsRequired();

        builder.Property(x => x.DeliveryFee)
            .HasColumnType("numeric(18,2)")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.PickupFee)
            .HasColumnType("numeric(18,2)")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.IsCustom).IsRequired();
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.ModifiedBy).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.ModifiedDate).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.LocationId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false AND \"LocationId\" IS NOT NULL");

        builder.HasIndex(x => new { x.TenantId, x.DisplayName })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(x => new { x.TenantId, x.IsActive });
    }
}
