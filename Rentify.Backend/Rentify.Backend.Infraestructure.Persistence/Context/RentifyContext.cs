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
            modelBuilder.ApplyConfiguration((IEntityTypeConfiguration<Core.Domain.Entities.Core.SystemEmailTemplate>)new EntityConfiguration.Core.SystemEmailTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new TenantEmailConfigurationConfiguration());
            #endregion

            modelBuilder.ApplyConfiguration(new VehicleBrandConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleModelConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleTypeConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleImageConfiguration());
            modelBuilder.ApplyConfiguration(new RentCarEntityConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleUnavailableDateConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerDocumentConfiguration());
            modelBuilder.ApplyConfiguration(new ReservationConfiguration());
            modelBuilder.ApplyConfiguration(new ReservationVehicleConfiguration());
            modelBuilder.ApplyConfiguration(new ReservationPaymentConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
        }

        #region Core Domain DbSets

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantSettings> TenantSettings { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Core.Domain.Entities.Core.SystemEmailTemplate> SystemEmailTemplates { get; set; }
        public DbSet<TenantEmailConfiguration> TenantEmailConfigurations { get; set; }

        #endregion

        public DbSet<VehicleBrand> VehicleBrands { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<RentCar> RentCars { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleImage> VehicleImages { get; set; }
        public DbSet<VehicleUnavailableDate> VehicleUnavailableDates { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerDocument> CustomerDocuments { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationVehicle> ReservationVehicles { get; set; }
        public DbSet<ReservationPayment> ReservationPayments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
    }
}
