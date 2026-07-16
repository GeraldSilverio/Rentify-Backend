using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetAssignedVehicleFeatures;

public sealed class GetAssignedVehicleFeaturesHandler
    : IRequestHandler<GetAssignedVehicleFeaturesQuery, ResultReponse<IReadOnlyCollection<VehicleFeatureAssignmentResponse>>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleFeatureAssignmentRepository _vehicleFeatureAssignmentRepository;

    public GetAssignedVehicleFeaturesHandler(
        IVehicleRepository vehicleRepository,
        IVehicleFeatureAssignmentRepository vehicleFeatureAssignmentRepository)
    {
        _vehicleRepository = vehicleRepository;
        _vehicleFeatureAssignmentRepository = vehicleFeatureAssignmentRepository;
    }

    public async Task<ResultReponse<IReadOnlyCollection<VehicleFeatureAssignmentResponse>>> Handle(
        GetAssignedVehicleFeaturesQuery request,
        CancellationToken cancellationToken)
    {
        if (!await _vehicleRepository.ExistsAsync(request.TenantId, request.VehicleId, cancellationToken))
            throw new ApiException("Vehículo no encontrado.", StatusCodes.Status404NotFound);

        IReadOnlyCollection<VehicleFeatureAssignment> assignments =
            await _vehicleFeatureAssignmentRepository.GetAssignmentsByVehicleWithFeaturesAsync(
            request.TenantId,
            request.VehicleId,
            cancellationToken);

        IReadOnlyCollection<VehicleFeatureAssignmentResponse> response = assignments
            .Select(assignment => new VehicleFeatureAssignmentResponse(
                assignment.VehicleFeature.Id,
                assignment.VehicleFeature.Name,
                assignment.VehicleFeature.Category))
            .OrderBy(feature => feature.Category)
            .ThenBy(feature => feature.Name)
            .ToList();

        return ResultReponse<IReadOnlyCollection<VehicleFeatureAssignmentResponse>>.Success(response);
    }
}
