using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration;

public class RentCarEntityConfiguration : IEntityTypeConfiguration<RentCar>
{
    public void Configure(EntityTypeBuilder<RentCar> builder)
    {
        builder.ToTable("RentCars");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(x => x.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);

            email.HasIndex(p => p.Value).IsUnique(true);
        });

        builder.OwnsOne(x => x.Phone, phone =>
        {
            phone.Property(x => x.Value)
                .HasColumnName("Phone")
                .IsRequired()
                .HasMaxLength(20);

            phone.HasIndex(p => p.Value).IsUnique(true);
        });

        builder.OwnsOne(x => x.WhatsApp, whatsapp =>
        {
            whatsapp.Property(x => x.Value)
                .HasColumnName("WhatsApp")
                .HasMaxLength(20);

            whatsapp.HasIndex(p => p.Value).IsUnique(true);
        });

        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(x => x.Street)
                .HasColumnName("Street")
                .IsRequired()
                .HasMaxLength(200);

            address.Property(x => x.City)
                .HasColumnName("City")
                .IsRequired()
                .HasMaxLength(100);

            address.Property(x => x.Country)
                .HasColumnName("Country")
                .IsRequired()
                .HasMaxLength(100);
        });

        
    }
}