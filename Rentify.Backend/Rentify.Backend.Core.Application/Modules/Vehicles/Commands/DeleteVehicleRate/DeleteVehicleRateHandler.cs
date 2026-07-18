using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleRate;

public sealed class DeleteVehicleRateHandler : IRequestHandler<DeleteVehicleRateCommand, ResultReponse<bool>>
{
    private readonly IVehicleRateRepository _vehicleRateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVehicleRateHandler(IVehicleRateRepository vehicleRateRepository, IUnitOfWork unitOfWork)
    {
        _vehicleRateRepository = vehicleRateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<bool>> Handle(DeleteVehicleRateCommand request, CancellationToken cancellationToken)
    {
        VehicleRate rate = await _vehicleRateRepository.GetByIdAsync(request.TenantId, request.VehicleId, request.RateId, cancellationToken)
            ?? throw new ApiException("Tarifa no encontrada.", StatusCodes.Status404NotFound);

        int activeRatesCount = await _vehicleRateRepository.CountActiveByVehicleAsync(
            request.TenantId,
            request.VehicleId,
            cancellationToken);

        if (activeRatesCount <= 1)
        {
            throw new ApiException(
                "El vehículo debe mantener al menos una tarifa activa.",
                StatusCodes.Status400BadRequest);
        }

        rate.Delete(request.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultReponse<bool>.Success(true);
    }
}
