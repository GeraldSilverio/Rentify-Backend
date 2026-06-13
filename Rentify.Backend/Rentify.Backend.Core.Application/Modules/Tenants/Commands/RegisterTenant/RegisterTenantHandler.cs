using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Application.Shared.Response;
using Rentify.Backend.Core.Application.Shared.Security;
using Rentify.Backend.Core.Application.Shared.UnitOfWork;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;

public sealed class RegisterTenantHandler : IRequestHandler<RegisterTenantCommand, ResultReponse<RegisterTenantResponse>>
{
    private readonly ITenantService _tenantService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IAccountService _accountService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterTenantHandler(
        ITenantService tenantService,
        ISubscriptionService subscriptionService,
        IAccountService accountService,
        IUnitOfWork unitOfWork)
    {
        _tenantService = tenantService;
        _subscriptionService = subscriptionService;
        _accountService = accountService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<RegisterTenantResponse>> Handle(
        RegisterTenantCommand request,
        CancellationToken cancellationToken)
    {
        if (await _accountService.ExistsByEmailAsync(request.OwnerEmail))
        {
            throw new ApiException("Owner email already exists", StatusCodes.Status400BadRequest);
        }

        var tenantId = await _tenantService.CreateTenantAsync(request, cancellationToken);
        var subscription = await _subscriptionService.RegisterSubscriptionAsync(tenantId, request, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var ownerUserId = await _accountService.CreateUserAsync(new CreateUserCommand(
            request.OwnerFullName,
            request.OwnerUserName,
            request.OwnerEmail,
            request.OwnerPassword,
            request.OwnerPhoneNumber,
            tenantId,
            request.CreatedBy,
            ApplicationRoles.Owner));

        return ResultReponse<RegisterTenantResponse>.Success(new RegisterTenantResponse(
            tenantId,
            subscription.Id,
            ownerUserId,
            subscription.ExpiresAt,
            subscription.TrialEndsAt,
            "Successfully registered tenant."));
    }
}
