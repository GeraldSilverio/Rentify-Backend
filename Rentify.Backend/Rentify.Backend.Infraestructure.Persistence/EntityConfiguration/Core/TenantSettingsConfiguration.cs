using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Core;

public sealed class TenantSettingsConfiguration : IEntityTypeConfiguration<TenantSettings>
{
    public void Configure(EntityTypeBuilder<TenantSettings> builder)
    {
        builder.ToTable("TenantSettings");

        builder.HasKey(x => x.TenantId);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.TimeZone)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Language)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.AllowOnlineReservations)
            .IsRequired();

        builder.Property(x => x.EnableNotifications)
            .IsRequired();

        builder.Property(x => x.EnableSmsNotifications)
            .IsRequired();

        builder.Property(x => x.EnableEmailNotifications)
            .IsRequired();

        builder.Property(x => x.LogoUrl)
            .HasMaxLength(500);

        builder.Property(x => x.PrimaryColor)
            .HasMaxLength(20);

        builder.Property(x => x.SecondaryColor)
            .HasMaxLength(20);

        // Audit
        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        builder.Property(x => x.ModifiedDate)
            .IsRequired();
    }
}