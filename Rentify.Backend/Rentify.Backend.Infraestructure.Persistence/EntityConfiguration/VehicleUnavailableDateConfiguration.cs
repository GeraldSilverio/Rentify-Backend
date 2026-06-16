using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration;

public sealed class VehicleUnavailableDateConfiguration : IEntityTypeConfiguration<VehicleUnavailableDate>
{
    public void Configure(EntityTypeBuilder<VehicleUnavailableDate> builder)
    {
        builder.ToTable("VehicleUnavailableDates");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.TenantId, x.VehicleId });

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(250);

        builder.HasIndex(x => new { x.VehicleId, x.StartDate, x.EndDate });
    }
}
