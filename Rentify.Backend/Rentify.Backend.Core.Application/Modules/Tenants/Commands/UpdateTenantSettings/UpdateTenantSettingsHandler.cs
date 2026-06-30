using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Domain.Entities.Core;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantSettings;

public sealed class UpdateTenantSettingsHandler
    : IRequestHandler<UpdateTenantSettingsCommand, ResultReponse<TenantSettingsResponse>>
{
    private readonly ITenantSettingRepository _tenantSettingRepository;
    private readonly ITenantReadRepository _tenantReadRepository;
    private readonly ITenantAccessService _tenantAccessService;
    private readonly ICurrentSubscriptionService _currentSubscriptionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTenantSettingsHandler> _logger;

    public UpdateTenantSettingsHandler(
        ITenantSettingRepository tenantSettingRepository,
        ITenantReadRepository tenantReadRepository,
        ITenantAccessService tenantAccessService,
        ICurrentSubscriptionService currentSubscriptionService,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTenantSettingsHandler> logger)
    {
        _tenantSettingRepository = tenantSettingRepository;
        _tenantReadRepository = tenantReadRepository;
        _tenantAccessService = tenantAccessService;
        _currentSubscriptionService = currentSubscriptionService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultReponse<TenantSettingsResponse>> Handle(
        UpdateTenantSettingsCommand request,
        CancellationToken cancellationToken)
    {
        Tenant tenant = await _tenantAccessService.GetActiveTenantAsync(request.TenantId, cancellationToken);

        TenantSettings settings = await _tenantSettingRepository.GetByTenantIdAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Tenant settings not found.", StatusCodes.Status404NotFound);

        ValidateBusinessModelRules(tenant.BusinessModel, request);
        await ValidateSubscriptionRulesAsync(request, cancellationToken);

        settings.Update(
            request.CurrencyCode,
            request.TimeZone,
            request.EnableReservations,
            request.EnableDriverFleet,
            request.EnableMaintenance,
            request.EnableLateFees,
            request.EnablePublicCatalog,
            request.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Tenant settings updated. TenantId: {TenantId}, ModifiedBy: {ModifiedBy}",
            request.TenantId,
            request.ModifiedBy);

        TenantSettingsResponse response = await _tenantReadRepository.GetSettingsAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Tenant settings not found.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantSettingsResponse>.Success(response);
    }

    private static void ValidateBusinessModelRules(
        BusinessModel businessModel,
        UpdateTenantSettingsCommand request)
    {
        bool supportsTraditionalRentCar = businessModel is BusinessModel.TraditionalRentCar or BusinessModel.Mixed;
        bool supportsDriverFleet = businessModel is BusinessModel.DriverFleetRental or BusinessModel.Mixed;

        if (request.EnableDriverFleet && !supportsDriverFleet)
            throw new ApiException("Driver fleet can only be enabled for DriverFleetRental or Mixed tenants.", StatusCodes.Status400BadRequest);

        if (request.EnablePublicCatalog && !supportsTraditionalRentCar)
            throw new ApiException("Public catalog can only be enabled for TraditionalRentCar or Mixed tenants.", StatusCodes.Status400BadRequest);

        if (request.EnableReservations && !supportsTraditionalRentCar)
            throw new ApiException("Reservations can only be enabled for TraditionalRentCar or Mixed tenants.", StatusCodes.Status400BadRequest);

        if (request.EnableLateFees && !supportsDriverFleet)
            throw new ApiException("Late fees can only be enabled for DriverFleetRental or Mixed tenants.", StatusCodes.Status400BadRequest);
    }

    private async Task ValidateSubscriptionRulesAsync(
        UpdateTenantSettingsCommand request,
        CancellationToken cancellationToken)
    {
        if (!request.EnableMaintenance)
            return;

        Subscription subscription = await _currentSubscriptionService.GetCurrentSubscriptionAsync(request.TenantId, cancellationToken);

        if (!subscription.SubscriptionPlan.MaintenanceModuleEnabled)
            throw new ApiException("The current subscription plan does not include the maintenance module.", StatusCodes.Status400BadRequest);

    }
}
