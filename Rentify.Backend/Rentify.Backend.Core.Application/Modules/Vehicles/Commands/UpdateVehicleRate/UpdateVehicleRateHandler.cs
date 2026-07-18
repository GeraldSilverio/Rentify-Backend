using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleRate;

public sealed class UpdateVehicleRateHandler : IRequestHandler<UpdateVehicleRateCommand, ResultReponse<VehicleRateResponse>>
{
    private readonly IVehicleRateRepository _vehicleRateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVehicleRateHandler(IVehicleRateRepository vehicleRateRepository, IUnitOfWork unitOfWork)
    {
        _vehicleRateRepository = vehicleRateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<VehicleRateResponse>> Handle(UpdateVehicleRateCommand request, CancellationToken cancellationToken)
    {
        VehicleRate rate = await _vehicleRateRepository.GetByIdAsync(request.TenantId, request.VehicleId, request.RateId, cancellationToken)
            ?? throw new ApiException("Tarifa no encontrada.", StatusCodes.Status404NotFound);

        rate.Update(request.Price, request.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultReponse<VehicleRateResponse>.Success(new VehicleRateResponse(rate.Id, rate.RentalType, rate.Price));
    }
}
