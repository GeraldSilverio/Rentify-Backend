using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.ValueObjects;

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

            builder.Property(x => x.Email)
            .HasConversion(
                 email => email.Value,
                 value => new Email(value))
                 .HasColumnName("Email")
                .HasMaxLength(150)
                .IsRequired();

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.PhoneNumber)
                .HasConversion(
                    phone => phone.Value,
                    value => new PhoneNumber(value))
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20)
                .IsRequired();


            builder.Property(x => x.WhatsApp)
                .HasConversion(
                    phone => phone.Value,
                    value => new PhoneNumber(value))
                .HasColumnName("WhatsApp")
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(x => x.WhatsApp)
                .IsUnique();

            builder.OwnsOne(x => x.Address, y =>
            {
                y.Property(x => x.Street)
                    .HasColumnName("Street")
                    .HasMaxLength(200)
                    .IsRequired();

                y.Property(x => x.City)
                    .HasColumnName("City")
                    .HasMaxLength(100)
                    .IsRequired();

                y.Property(x => x.Country)
                    .HasColumnName("Country")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.Property(x => x.BusinessModel)
                .HasConversion<int>()
                .IsRequired();
        }
    }
}