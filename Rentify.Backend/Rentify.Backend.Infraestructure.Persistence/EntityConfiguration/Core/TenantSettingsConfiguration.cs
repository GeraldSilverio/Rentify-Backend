using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Core;

public sealed class TenantSettingsConfiguration : IEntityTypeConfiguration<TenantSettings>
{
    public void Configure(EntityTypeBuilder<TenantSettings> builder)
    {
        builder.ToTable("TenantSettings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.CurrencyCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.TimeZone)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.EnableReservations)
            .IsRequired();

        builder.Property(x => x.EnableDriverFleet)
            .IsRequired();

        builder.Property(x => x.EnableMaintenance)
            .IsRequired();

        builder.Property(x => x.EnableLateFees)
            .IsRequired();

        builder.Property(x => x.EnablePublicCatalog)
            .IsRequired();

        builder.HasOne(x => x.Tenant)
            .WithOne()
            .HasForeignKey<TenantSettings>(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TenantId)
            .IsUnique();
    }
}