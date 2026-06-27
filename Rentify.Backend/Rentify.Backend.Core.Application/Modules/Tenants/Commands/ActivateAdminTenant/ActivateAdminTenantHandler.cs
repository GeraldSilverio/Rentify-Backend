using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Tenants.Dtos;
using Rentify.Backend.Core.Domain.Entities.Core;

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.ActivateAdminTenant;

public sealed class ActivateAdminTenantHandler
    : IRequestHandler<ActivateAdminTenantCommand, ResultReponse<AdminTenantDetailResponse>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantReadRepository _tenantReadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateAdminTenantHandler> _logger;

    public ActivateAdminTenantHandler(
        ITenantRepository tenantRepository,
        ITenantReadRepository tenantReadRepository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateAdminTenantHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _tenantReadRepository = tenantReadRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultReponse<AdminTenantDetailResponse>> Handle(
        ActivateAdminTenantCommand request,
        CancellationToken cancellationToken)
    {
        Tenant tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Tenant not found.", StatusCodes.Status404NotFound);

        if (!tenant.IsActive)
        {
            tenant.Activate(request.ModifiedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Admin tenant activated. TenantId: {TenantId}, ModifiedBy: {ModifiedBy}",
                request.TenantId,
                request.ModifiedBy);
        }
        else
        {
            _logger.LogInformation(
                "Admin tenant activate requested for already active tenant. TenantId: {TenantId}, ModifiedBy: {ModifiedBy}",
                request.TenantId,
                request.ModifiedBy);
        }

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
