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

namespace Rentify.Backend.Core.Application.Modules.Tenants.Commands.UpdateTenantProfile;

public sealed class UpdateTenantProfileHandler
    : IRequestHandler<UpdateTenantProfileCommand, ResultReponse<TenantProfileResponse>>
{
    private readonly ITenantReadRepository _tenantReadRepository;
    private readonly ITenantAccessService _tenantAccessService;
    private readonly ITenantUniquenessService _tenantUniquenessService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTenantProfileHandler> _logger;

    public UpdateTenantProfileHandler(
        ITenantReadRepository tenantReadRepository,
        ITenantAccessService tenantAccessService,
        ITenantUniquenessService tenantUniquenessService,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTenantProfileHandler> logger)
    {
        _tenantReadRepository = tenantReadRepository;
        _tenantAccessService = tenantAccessService;
        _tenantUniquenessService = tenantUniquenessService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultReponse<TenantProfileResponse>> Handle(
        UpdateTenantProfileCommand request,
        CancellationToken cancellationToken)
    {
        Tenant tenant = await _tenantAccessService.GetActiveTenantAsync(request.TenantId, cancellationToken);

        await _tenantUniquenessService.EnsureRncIsUniqueAsync(
            request.Rnc,
            request.TenantId,
            cancellationToken);

        tenant.Update(
            request.Name,
            request.LegalName,
            request.Rnc,
            tenant.BusinessModel,
            request.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Tenant profile updated. TenantId: {TenantId}, ModifiedBy: {ModifiedBy}",
            request.TenantId,
            request.ModifiedBy);

        TenantProfileResponse response = await _tenantReadRepository.GetProfileAsync(request.TenantId, cancellationToken)
            ?? throw new ApiException("Tenant not found.", StatusCodes.Status404NotFound);

        return ResultReponse<TenantProfileResponse>.Success(response);
    }
}
