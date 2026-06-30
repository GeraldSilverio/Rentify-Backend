using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Application.Modules.Tenants.Validation;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateAdminTenant;

public sealed class UpdateAdminTenantHandler
    : IRequestHandler<UpdateAdminTenantCommand, ResultReponse<AdminTenantDetailResponse>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantReadRepository _tenantReadRepository;
    private readonly ITenantUniquenessService _tenantUniquenessService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAdminTenantHandler> _logger;

    public UpdateAdminTenantHandler(
        ITenantRepository tenantRepository,
        ITenantReadRepository tenantReadRepository,
        ITenantUniquenessService tenantUniquenessService,
        IUnitOfWork unitOfWork,
        ILogger<UpdateAdminTenantHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _tenantReadRepository = tenantReadRepository;
        _tenantUniquenessService = tenantUniquenessService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultReponse<AdminTenantDetailResponse>> Handle(
        UpdateAdminTenantCommand request,
        CancellationToken cancellationToken)
    {
        Tenant tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Tenant not found.", StatusCodes.Status404NotFound);

        await _tenantUniquenessService.EnsureRncIsUniqueAsync(
            request.Rnc,
            request.TenantId,
            cancellationToken);

        tenant.Update(
            request.Name,
            request.LegalName,
            TenantRncRules.NormalizeOrNull(request.Rnc),
            request.BusinessModel!.Value,
            request.ModifiedBy);

        // TODO: Synchronize TenantSettings and PaymentPolicy when changing BusinessModel if a dedicated flow is required.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Admin tenant updated. TenantId: {TenantId}, BusinessModel: {BusinessModel}, ModifiedBy: {ModifiedBy}",
            request.TenantId,
            request.BusinessModel,
            request.ModifiedBy);

        AdminTenantDetailResponse response = await BuildDetailAsync(request.TenantId, cancellationToken);
        return ResultReponse<AdminTenantDetailResponse>.Success(response);
    }

    private async Task<AdminTenantDetailResponse> BuildDetailAsync(
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        TenantProfileResponse profile = await _tenantReadRepository.GetProfileAsync(tenantId, cancellationToken)
            ?? throw new ApiException("Tenant not found.", StatusCodes.Status404NotFound);

        TenantUsageResponse usage = await _tenantReadRepository.GetUsageAsync(tenantId, cancellationToken);

        return new AdminTenantDetailResponse(profile, usage);
    }
}
