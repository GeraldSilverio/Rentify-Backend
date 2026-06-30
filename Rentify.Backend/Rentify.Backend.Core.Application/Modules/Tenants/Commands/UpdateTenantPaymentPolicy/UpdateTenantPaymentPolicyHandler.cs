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
using Rentify.Backend.Core.Domain.Entities.Payments;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantPaymentPolicy;

public sealed class UpdateTenantPaymentPolicyHandler
    : IRequestHandler<UpdateTenantPaymentPolicyCommand, ResultReponse<TenantPaymentPolicyResponse>>
{
    private readonly IPaymentPolicyRepository _paymentPolicyRepository;
    private readonly ITenantSettingRepository _tenantSettingRepository;
    private readonly ITenantReadRepository _tenantReadRepository;
    private readonly ITenantAccessService _tenantAccessService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTenantPaymentPolicyHandler> _logger;

    public UpdateTenantPaymentPolicyHandler(
        IPaymentPolicyRepository paymentPolicyRepository,
        ITenantSettingRepository tenantSettingRepository,
        ITenantReadRepository tenantReadRepository,
        ITenantAccessService tenantAccessService,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTenantPaymentPolicyHandler> logger)
    {
        _paymentPolicyRepository = paymentPolicyRepository;
        _tenantSettingRepository = tenantSettingRepository;
        _tenantReadRepository = tenantReadRepository;
        _tenantAccessService = tenantAccessService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultReponse<TenantPaymentPolicyResponse>> Handle(
        UpdateTenantPaymentPolicyCommand request,
        CancellationToken cancellationToken)
    {
        await _tenantAccessService.EnsureTenantIsActiveAsync(request.TenantId, cancellationToken);

        PaymentPolicy paymentPolicy = await _paymentPolicyRepository.GetDefaultByTenantIdAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Default payment policy not found.", StatusCodes.Status404NotFound);

        TenantSettings settings = await _tenantSettingRepository.GetByTenantIdAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Tenant settings not found.", StatusCodes.Status404NotFound);

        if (request.LateFeeEnabled && !settings.EnableLateFees)
            throw new ApiException("Late fees must be enabled in tenant settings before enabling them in the payment policy.", StatusCodes.Status400BadRequest);

        paymentPolicy.Update(
            request.Name,
            request.PaymentFrequency,
            request.CutoffDayOfWeek,
            request.GraceDays,
            request.ReminderStartDayOfWeek,
            request.LateFeeEnabled,
            isDefault: true,
            request.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Tenant payment policy updated. TenantId: {TenantId}, PaymentPolicyId: {PaymentPolicyId}, ModifiedBy: {ModifiedBy}",
            request.TenantId,
            paymentPolicy.Id,
            request.ModifiedBy);

        TenantPaymentPolicyResponse response = await _tenantReadRepository.GetDefaultPaymentPolicyAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Default payment policy not found.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantPaymentPolicyResponse>.Success(response);
    }
}
