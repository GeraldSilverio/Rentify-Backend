using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Events;
using Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser;
using Rentify.Backend.Core.Application.Shared.Constants;
using Rentify.Backend.Core.Application.Shared.Contracts;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Application.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Implementations.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboxService _outboxService;
        public TenantService(ITenantRepository tenantRepository, ISubscriptionService subscriptionService, IAccountService accountService, IUnitOfWork unitOfWork, IOutboxService outboxService)
        {
            _tenantRepository = tenantRepository;
            _subscriptionService = subscriptionService;
            _accountService = accountService;
            _unitOfWork = unitOfWork;
            _outboxService = outboxService;
        }

        public async Task<RegisterTenantResponse> CreateTenantAsync(RegisterTenantCommand request, CancellationToken cancellationToken)
        {
            Tenant tenant = Tenant.Create(request.Name, request.LegalName, request.Rnc, request.BusinessModel, request.CreatedBy);

            await _tenantRepository.AddAsync(tenant, cancellationToken);

            Subscription subscription = await _subscriptionService.RegisterSubscriptionAsync(tenant.Id, request, cancellationToken);

            Guid ownerUserId = await _accountService.CreateUserAsync(new CreateUserCommand(
               request.UserInformation.FullName,
               request.UserInformation.UserName,
               request.ContactInformation.Email,
               request.UserInformation.Password,
               request.ContactInformation.PhoneNumber,
               tenant.Id,
               request.CreatedBy,
               ApplicationRoles.Owner));

            await _outboxService.AddAsync(
                tenant.Id,
                OutboxMessageTypes.TenantRegistered,
                new TenantRegisteredOutboxPayload(
                    tenant.Id,
                    subscription.Id,
                    ownerUserId,
                    request.Name,
                    request.UserInformation.FullName,
                    request.UserInformation.ContactInformation.Email,
                    request.SubscriptionPlanCode,
                    subscription.Status.ToString(),
                    subscription.StartsAt,
                    subscription.ExpiresAt,
                    subscription.TrialEndsAt,
                    request.BusinessModel),
                request.CreatedBy,
                cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterTenantResponse(tenant.Id, subscription.Id, ownerUserId, subscription.ExpiresAt, subscription.TrialEndsAt);
        }
    }
}
