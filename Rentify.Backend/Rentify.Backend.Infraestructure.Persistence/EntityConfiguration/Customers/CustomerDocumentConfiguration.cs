using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Customers;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Customers;

public sealed class CustomerDocumentConfiguration : IEntityTypeConfiguration<CustomerDocument>
{
    public void Configure(EntityTypeBuilder<CustomerDocument> builder)
    {
        builder.ToTable("CustomerDocuments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Url).IsRequired().HasMaxLength(500);
        builder.Property(x => x.PublicId).IsRequired().HasMaxLength(255);
        builder.Property(x => x.DocumentType).HasConversion<string>().HasMaxLength(40);

        builder.HasIndex(x => new { x.TenantId, x.CustomerId });
        builder.HasIndex(x => x.PublicId).IsUnique();
    }
}
