using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Core
{
    public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Slug)
                .IsUnique();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.IsSuspended)
                .IsRequired();

            builder.Property(x => x.SuspendedAt);

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

            // Relationships
            builder.HasOne(x => x.Settings)
                .WithOne(x => x.Tenant)
                .HasForeignKey<TenantSettings>(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}