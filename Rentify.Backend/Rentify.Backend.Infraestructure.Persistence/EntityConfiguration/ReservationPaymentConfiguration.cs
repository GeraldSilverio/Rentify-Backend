using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration;

public sealed class ReservationPaymentConfiguration : IEntityTypeConfiguration<ReservationPayment>
{
    public void Configure(EntityTypeBuilder<ReservationPayment> builder)
    {
        builder.ToTable("ReservationPayments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Method).HasConversion<string>().HasMaxLength(40);
        builder.Property(x => x.Reference).HasMaxLength(100);

        builder.HasIndex(x => new { x.TenantId, x.ReservationId });
    }
}
