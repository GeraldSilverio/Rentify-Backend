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

            builder.Property(x => x.LegalName)
               .IsRequired()
               .HasMaxLength(150);

            builder.Property(x => x.Rnc)
                .IsRequired()
                .HasMaxLength(15);

            builder.HasIndex(x => x.Rnc)
                .IsUnique();

            builder.Property(x => x.IsActive)
                .IsRequired();

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

            builder.Property(x => x.BusinessModel)
                .HasConversion<int>()
                .IsRequired();


        }
    }
}