using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Queries.GetSubscriptionPlans;
using Rentify.Backend.Infraestructure.Persistence.Context;

namespace Rentify.Backend.Infraestructure.Persistence.Repositories
{
    public sealed class SubscriptionPlanRepository(RentifyContext rentifyContext) : ISubscriptionPlanRepository
    {
        private readonly RentifyContext _rentifyContext = rentifyContext;

        public async Task<IEnumerable<GetSubscriptionPlanResponse>> GetSubscriptionPlansAsync()
        {
            try
            {

                return await _rentifyContext.SubscriptionPlans
                    .Where(x => x.IsActive && !x.IsDeleted).
                    Select(x => new GetSubscriptionPlanResponse(
                    x.Id,
                    x.Name,
                    x.Price,
                    x.MaxVehicles,
                    x.MaxEmployees,
                    x.MaxBranches,
                    x.MaxReservationsPerMonth,
                    x.MultiBranchEnabled,
                    x.ReportsEnabled,
                    x.ContractsEnabled,
                    x.MaintenanceModuleEnabled,
                    x.PrioritySupportEnabled,
                    x.WhiteLabelEnabled))
                    .AsNoTracking()
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving subscription plans: {ex.Message}", ex);
            }
        }
    }
}
