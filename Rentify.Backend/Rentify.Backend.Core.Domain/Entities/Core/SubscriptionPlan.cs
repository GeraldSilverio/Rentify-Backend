using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Core
{
    public class SubscriptionPlan : BaseEntity
    {
        public Guid Id { get; private set; }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public PlanType Type { get; private set; }

        public BillingCycle BillingCycle { get; private set; }

        public decimal Price { get; private set; }

        // Limits
        public int MaxVehicles { get; private set; }

        public int MaxEmployees { get; private set; }

        public int MaxBranches { get; private set; }

        public int MaxReservationsPerMonth { get; private set; }

        // Features
        public bool MultiBranchEnabled { get; private set; }

        public bool ReportsEnabled { get; private set; }

        public bool ApiAccessEnabled { get; private set; }

        public bool ContractsEnabled { get; private set; }

        public bool MaintenanceModuleEnabled { get; private set; }

        public bool PrioritySupportEnabled { get; private set; }

        public bool WhiteLabelEnabled { get; private set; }

        // EF
        private SubscriptionPlan()
        {
        }
        public static SubscriptionPlan Starter()
        {
            return new SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                Code = "STARTER",
                Name = "Starter",
                Type = PlanType.Starter,
                BillingCycle = BillingCycle.Monthly,
                Price = 29,

                MaxVehicles = 5,
                MaxEmployees = 2,
                MaxBranches = 1,
                MaxReservationsPerMonth = 50,

                MultiBranchEnabled = false,
                ReportsEnabled = false,
                ApiAccessEnabled = false,
                ContractsEnabled = false,
                MaintenanceModuleEnabled = false,
                PrioritySupportEnabled = false,
                WhiteLabelEnabled = false,

                IsActive = true
            };
        }

        public static SubscriptionPlan Pro()
        {
            return new SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                Code = "PRO",
                Name = "Pro",
                Type = PlanType.Pro,
                BillingCycle = BillingCycle.Monthly,
                Price = 99,

                MaxVehicles = 50,
                MaxEmployees = 20,
                MaxBranches = 5,
                MaxReservationsPerMonth = 1000,

                MultiBranchEnabled = true,
                ReportsEnabled = true,
                ApiAccessEnabled = false,
                ContractsEnabled = true,
                MaintenanceModuleEnabled = true,
                PrioritySupportEnabled = true,
                WhiteLabelEnabled = false,

                IsActive = true
            };
        }
    }
}
