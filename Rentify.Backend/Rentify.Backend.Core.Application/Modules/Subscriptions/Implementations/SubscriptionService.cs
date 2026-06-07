// using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Repositories;
// using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;
// using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;
// using Rentify.Backend.Core.Domain.Entities.Core;
//
// namespace Rentify.Backend.Core.Application.Modules.Subscriptions.Implementations;
//
// public class SubscriptionService : ISubscriptionService
// {
//     private readonly ISubscriptionRepository _subscriptionRepository;
//
//     public SubscriptionService(ISubscriptionRepository subscriptionRepository)
//     {
//         _subscriptionRepository = subscriptionRepository;
//     }
//
//     public async Task RegisterSubscriptionAsync(Guid planId,Guid tenantId,RegisterTenantCommand command,CancellationToken cancellationToken)
//     {
//         var subscription = Subscription.Create(
//             tenantId,
//             planId,
//             command.DaysSubscriptions,
//             command.Email);
//
//         await _subscriptionRepository.AddAsync(
//             subscription,
//             cancellationToken);
//     }
// }