using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Events
{
    public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TenantId)
                .IsRequired(false);

            builder.Property(x => x.Type)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.Payload)
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.RetryCount)
                .IsRequired();

            builder.Property(x => x.MaxRetries)
                .IsRequired();

            builder.Property(x => x.ProcessedDate)
                .IsRequired(false);

            builder.Property(x => x.NextRetryDate)
                .IsRequired(false);

            builder.Property(x => x.Error)
                .HasMaxLength(4000)
                .IsRequired(false);

            builder.Property(x => x.CorrelationId)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.ModifiedBy)
                .HasMaxLength(150)
                .IsRequired();

            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.TenantId);
            builder.HasIndex(x => x.CreatedDate);
            builder.HasIndex(x => new { x.Status, x.NextRetryDate });
        }
    }
}
