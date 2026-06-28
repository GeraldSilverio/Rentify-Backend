using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Contracts;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Events;
using Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.Entities.Payments;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantSettingService _tenantSettingService;
        private readonly IPaymentPolicyService _paymentPolicyService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboxService _outboxService;
        public TenantService(ITenantRepository tenantRepository, ISubscriptionService subscriptionService, IAccountService accountService, IUnitOfWork unitOfWork, IOutboxService outboxService, ITenantSettingService tenantSettingService, IPaymentPolicyService paymentPolicyService)
        {
            _tenantRepository = tenantRepository;
            _subscriptionService = subscriptionService;
            _accountService = accountService;
            _unitOfWork = unitOfWork;
            _outboxService = outboxService;
            _tenantSettingService = tenantSettingService;
            _paymentPolicyService = paymentPolicyService;
        }

        public async Task<RegisterTenantResponse> CreateTenantAsync(RegisterTenantCommand request, CancellationToken cancellationToken)
        {
            Tenant tenant = Tenant.Create(request.Name, request.LegalName, request.Rnc, request.BusinessModel, request.CreatedBy);

            await _tenantRepository.AddAsync(tenant, cancellationToken);

            await _tenantSettingService.AddAsync(TenantSettings.CreateDefault(tenant.Id, request.BusinessModel, request.CreatedBy));

            await _paymentPolicyService.AddAsync(PaymentPolicy.CreateDefault(tenant.Id, request.BusinessModel, request.CreatedBy),cancellationToken);
           
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
