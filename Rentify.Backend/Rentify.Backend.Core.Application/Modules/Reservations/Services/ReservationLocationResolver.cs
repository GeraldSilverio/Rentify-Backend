using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Locations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Domain.Entities.Locations;

namespace Rentify.Backend.Core.Application.Modules.Reservations.Services;

public sealed class ReservationLocationResolver : IReservationLocationResolver
{
    private readonly ITenantLocationRepository _tenantLocationRepository;

    public ReservationLocationResolver(ITenantLocationRepository tenantLocationRepository)
    {
        _tenantLocationRepository = tenantLocationRepository;
    }

    public async Task<ResolvedReservationLocation> ResolveDeliveryAsync(
        Guid tenantId,
        Guid? tenantLocationId,
        string? customName,
        decimal customFee,
        CancellationToken cancellationToken)
    {
        if (!tenantLocationId.HasValue)
        {
            return new ResolvedReservationLocation(
                null,
                RequireCustomName(customName),
                customFee);
        }

        TenantLocation location =
            await _tenantLocationRepository.GetByIdAsync(
                tenantId,
                tenantLocationId.Value,
                cancellationToken)
            ?? throw new ApiException(
                "La ubicación de entrega no está disponible.",
                StatusCodes.Status400BadRequest);

        if (!location.IsActive)
        {
            throw new ApiException(
                "La ubicación de entrega no está disponible.",
                StatusCodes.Status400BadRequest);
        }

        if (!location.AllowsDelivery)
        {
            throw new ApiException(
                "La ubicación de entrega no permite entregas.",
                StatusCodes.Status400BadRequest);
        }

        return new ResolvedReservationLocation(
            location.Id,
            location.DisplayName,
            location.DeliveryFee);
    }

    public async Task<ResolvedReservationLocation> ResolveReturnAsync(
        Guid tenantId,
        Guid? tenantLocationId,
        string? customName,
        decimal customFee,
        CancellationToken cancellationToken)
    {
        if (!tenantLocationId.HasValue)
        {
            return new ResolvedReservationLocation(
                null,
                RequireCustomName(customName),
                customFee);
        }

        TenantLocation location =
            await _tenantLocationRepository.GetByIdAsync(
                tenantId,
                tenantLocationId.Value,
                cancellationToken)
            ?? throw new ApiException(
                "La ubicación de devolución no está disponible.",
                StatusCodes.Status400BadRequest);

        if (!location.IsActive)
        {
            throw new ApiException(
                "La ubicación de devolución no está disponible.",
                StatusCodes.Status400BadRequest);
        }

        if (!location.AllowsPickup)
        {
            throw new ApiException(
                "La ubicación de devolución no permite recogidas.",
                StatusCodes.Status400BadRequest);
        }

        return new ResolvedReservationLocation(
            location.Id,
            location.DisplayName,
            location.PickupFee);
    }

    private static string RequireCustomName(string? customName)
    {
        if (string.IsNullOrWhiteSpace(customName))
        {
            throw new ApiException(
                "El nombre de la ubicación es requerido.",
                StatusCodes.Status400BadRequest);
        }

        return customName;
    }
}
