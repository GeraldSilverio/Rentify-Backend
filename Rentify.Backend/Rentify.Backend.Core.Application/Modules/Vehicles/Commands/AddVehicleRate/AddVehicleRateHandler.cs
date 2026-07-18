using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.AddVehicleRate;

public sealed class AddVehicleRateHandler : IRequestHandler<AddVehicleRateCommand, ResultReponse<VehicleRateResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleRateRepository _vehicleRateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddVehicleRateHandler(IVehicleRepository vehicleRepository, IVehicleRateRepository vehicleRateRepository, IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _vehicleRateRepository = vehicleRateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<VehicleRateResponse>> Handle(AddVehicleRateCommand request, CancellationToken cancellationToken)
    {
        if (await _vehicleRepository.GetByIdAsync(request.TenantId, request.VehicleId, cancellationToken) is null)
            throw new ApiException("Vehículo no encontrado.", StatusCodes.Status404NotFound);

        if (await _vehicleRateRepository.ExistsByRentalTypeAsync(
                request.TenantId,
                request.VehicleId,
                request.RentalType,
                cancellationToken: cancellationToken))
        {
            throw new ApiException("Ya existe una tarifa activa para este tipo de renta.", StatusCodes.Status400BadRequest);
        }

        VehicleRate rate = VehicleRate.Create(
            request.TenantId,
            request.VehicleId,
            request.RentalType,
            request.Price,
            request.CreatedBy);

        await _vehicleRateRepository.AddAsync(rate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultReponse<VehicleRateResponse>.Success(new VehicleRateResponse(rate.Id, rate.RentalType, rate.Price));
    }
}
