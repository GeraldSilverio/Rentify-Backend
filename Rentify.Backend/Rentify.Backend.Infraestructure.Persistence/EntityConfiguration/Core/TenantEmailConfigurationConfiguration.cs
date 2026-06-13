using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Core
{
    public sealed class TenantEmailConfigurationConfiguration : IEntityTypeConfiguration<TenantEmailConfiguration>
    {
        public void Configure(EntityTypeBuilder<TenantEmailConfiguration> builder)
        {
            builder.ToTable("TenantEmailConfigurations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Provider)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.ApiKey)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.FromEmail)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.FromName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.IsDefault)
                .IsRequired();

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

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.HasIndex(x => new { x.TenantId, x.Provider })
                .IsUnique();

            builder.HasIndex(x => new { x.TenantId, x.IsDefault });

            builder.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
