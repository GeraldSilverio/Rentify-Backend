using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleFeatures;

public sealed class UpdateVehicleFeaturesHandler
    : IRequestHandler<UpdateVehicleFeaturesCommand, ResultReponse<IReadOnlyCollection<VehicleFeatureAssignmentResponse>>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleFeatureRepository _vehicleFeatureRepository;
    private readonly IVehicleFeatureAssignmentRepository _vehicleFeatureAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVehicleFeaturesHandler(
        IVehicleRepository vehicleRepository,
        IVehicleFeatureRepository vehicleFeatureRepository,
        IVehicleFeatureAssignmentRepository vehicleFeatureAssignmentRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _vehicleFeatureRepository = vehicleFeatureRepository;
        _vehicleFeatureAssignmentRepository = vehicleFeatureAssignmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<IReadOnlyCollection<VehicleFeatureAssignmentResponse>>> Handle(
        UpdateVehicleFeaturesCommand request,
        CancellationToken cancellationToken)
    {
        if (!await _vehicleRepository.ExistsAsync(request.TenantId, request.VehicleId, cancellationToken))
            throw new ApiException("Vehículo no encontrado.", StatusCodes.Status404NotFound);

        if (request.FeatureIds.Any(id => id == Guid.Empty))
            throw new ApiException("Una o más características no son válidas.", StatusCodes.Status400BadRequest);

        HashSet<Guid> requestedFeatureIds = request.FeatureIds.ToHashSet();

        IReadOnlyCollection<VehicleFeature> validFeatures = requestedFeatureIds.Count == 0
            ? Array.Empty<VehicleFeature>()
            : await _vehicleFeatureRepository.GetActiveByIdsAsync(requestedFeatureIds.ToArray(), cancellationToken);

        if (validFeatures.Count != requestedFeatureIds.Count)
            throw new ApiException("Una o más características no son válidas.", StatusCodes.Status400BadRequest);

        IReadOnlyCollection<VehicleFeatureAssignment> currentAssignments =
            await _vehicleFeatureAssignmentRepository.GetAssignmentsByVehicleAsync(
                request.TenantId,
                request.VehicleId,
                cancellationToken);

        HashSet<Guid> currentFeatureIds = currentAssignments
            .Select(assignment => assignment.VehicleFeatureId)
            .ToHashSet();

        List<VehicleFeatureAssignment> assignmentsToAdd = requestedFeatureIds
            .Except(currentFeatureIds)
            .Select(featureId => VehicleFeatureAssignment.Create(
                request.TenantId,
                request.VehicleId,
                featureId,
                request.ModifiedBy))
            .ToList();

        List<VehicleFeatureAssignment> assignmentsToRemove = currentAssignments
            .Where(assignment => !requestedFeatureIds.Contains(assignment.VehicleFeatureId))
            .ToList();

        if (assignmentsToRemove.Count > 0)
            _vehicleFeatureAssignmentRepository.RemoveRange(assignmentsToRemove);

        if (assignmentsToAdd.Count > 0)
            await _vehicleFeatureAssignmentRepository.AddRangeAsync(assignmentsToAdd, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        IReadOnlyCollection<VehicleFeatureAssignment> finalAssignments =
            await _vehicleFeatureAssignmentRepository.GetAssignmentsByVehicleWithFeaturesAsync(
                request.TenantId,
                request.VehicleId,
                cancellationToken);

        IReadOnlyCollection<VehicleFeatureAssignmentResponse> response = finalAssignments
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
