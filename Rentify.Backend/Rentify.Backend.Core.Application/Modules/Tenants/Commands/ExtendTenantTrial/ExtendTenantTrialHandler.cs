using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Subscriptions.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.ExtendTenantTrial;

public sealed class ExtendTenantTrialHandler
    : IRequestHandler<ExtendTenantTrialCommand, ResultReponse<TenantSubscriptionResponse>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ITenantReadRepository _tenantReadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExtendTenantTrialHandler> _logger;

    public ExtendTenantTrialHandler(
        ITenantRepository tenantRepository,
        ISubscriptionRepository subscriptionRepository,
        ITenantReadRepository tenantReadRepository,
        IUnitOfWork unitOfWork,
        ILogger<ExtendTenantTrialHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _subscriptionRepository = subscriptionRepository;
        _tenantReadRepository = tenantReadRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultReponse<TenantSubscriptionResponse>> Handle(
        ExtendTenantTrialCommand request,
        CancellationToken cancellationToken)
    {
        _ = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Tenant not found.", StatusCodes.Status404NotFound);

        Subscription subscription = await _subscriptionRepository.GetCurrentByTenantIdAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Subscription not found.", StatusCodes.Status404NotFound);

        if (subscription.Status == SubscriptionStatus.Cancelled)
            throw new ApiException("Cancelled subscriptions cannot have their trial extended.", StatusCodes.Status400BadRequest);

        if (subscription.Status == SubscriptionStatus.Expired)
            throw new ApiException("Expired subscriptions cannot have their trial extended in this phase.", StatusCodes.Status400BadRequest);

        subscription.ExtendTrial(request.DaysToAdd, request.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Tenant trial extended. TenantId: {TenantId}, SubscriptionId: {SubscriptionId}, DaysToAdd: {DaysToAdd}, ModifiedBy: {ModifiedBy}",
            request.TenantId,
            subscription.Id,
            request.DaysToAdd,
            request.ModifiedBy);

        TenantSubscriptionResponse response = await _tenantReadRepository.GetCurrentSubscriptionAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Subscription not found.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantSubscriptionResponse>.Success(response);
    }
}
