using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Infraestructure.Persistence.EntityConfiguration;
using Rentify.Backend.Infraestructure.Persistence.EntityConfiguration.Core;

namespace Rentify.Backend.Infraestructure.Persistence.Context
{
    public class RentifyContext : DbContext
    {
        public RentifyContext(DbContextOptions<RentifyContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Core Domain Configurations
            modelBuilder.ApplyConfiguration(new TenantConfiguration());
            modelBuilder.ApplyConfiguration(new TenantSettingsConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionPlanConfiguration());
            modelBuilder.ApplyConfiguration(new EmailTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new TenantEmailConfigurationConfiguration());
            #endregion

            modelBuilder.ApplyConfiguration(new RentCarEntityConfiguration());
        }

        #region Core Domain DbSets

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantSettings> TenantSettings { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<TenantEmailConfiguration> TenantEmailConfigurations { get; set; }

        #endregion

        public DbSet<RentCar> RentCars { get; set; }
    }
}
