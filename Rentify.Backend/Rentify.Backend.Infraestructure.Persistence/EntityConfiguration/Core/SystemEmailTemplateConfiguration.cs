using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Core
{
    public sealed class SystemEmailTemplateConfiguration : IEntityTypeConfiguration<SystemEmailTemplate>
    {
        public void Configure(EntityTypeBuilder<SystemEmailTemplate> builder)
        {
            builder.ToTable("SystemEmailTemplates");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Subject)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.HtmlBody)
                .IsRequired();

            builder.Property(x => x.TextBody);

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
        }
    }
}
