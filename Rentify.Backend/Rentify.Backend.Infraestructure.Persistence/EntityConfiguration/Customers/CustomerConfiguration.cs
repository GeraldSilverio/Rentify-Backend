using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Customers;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Customers;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerType).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.IdentificationType).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.IdentificationNumber).IsRequired().HasMaxLength(30);
        builder.Property(x => x.IdentificationNumberNormalized).IsRequired().HasMaxLength(30);
        builder.Property(x => x.BirthDate).IsRequired(false);
        builder.Property(x => x.IsVerified).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.VerifiedAt).IsRequired(false);
        builder.Property(x => x.VerifiedBy).IsRequired(false);
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
        builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(30);
        builder.Property(x => x.AddressLine).HasMaxLength(250);
        builder.Property(x => x.Sector).HasMaxLength(100);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Province).HasMaxLength(100);
        builder.Property(x => x.AddressReference).HasMaxLength(250);

        builder.HasIndex(x => new { x.TenantId, x.Email });
        builder.HasIndex(x => new { x.TenantId, x.PhoneNumber });
        builder.HasIndex(x => new { x.TenantId, x.IdentificationType, x.IdentificationNumberNormalized })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasMany(x => x.Documents)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Documents)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
