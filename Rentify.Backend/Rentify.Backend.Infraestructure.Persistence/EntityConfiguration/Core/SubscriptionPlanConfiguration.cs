using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Core;

public sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlans");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.BillingCycle)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        // Limits
        builder.Property(x => x.MaxVehicles)
            .IsRequired();

        builder.Property(x => x.MaxEmployees)
            .IsRequired();

        builder.Property(x => x.MaxBranches)
            .IsRequired();

        builder.Property(x => x.MaxReservationsPerMonth)
            .IsRequired();

        // Features
        builder.Property(x => x.MultiBranchEnabled)
            .IsRequired();

        builder.Property(x => x.ReportsEnabled)
            .IsRequired();

        builder.Property(x => x.ApiAccessEnabled)
            .IsRequired();

        builder.Property(x => x.ContractsEnabled)
            .IsRequired();

        builder.Property(x => x.MaintenanceModuleEnabled)
            .IsRequired();

        builder.Property(x => x.PrioritySupportEnabled)
            .IsRequired();

        builder.Property(x => x.WhiteLabelEnabled)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        // Audit
        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);
    }
}