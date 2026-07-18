using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.Storage;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleImage;

public sealed class DeleteVehicleImageHandler
    : IRequestHandler<DeleteVehicleImageCommand, ResultReponse<bool>>
{
    private readonly IVehicleImageRepository _vehicleImageRepository;
    private readonly ICurrentRequestContext _currentRequestContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public DeleteVehicleImageHandler(IVehicleImageRepository vehicleImageRepository, ICurrentRequestContext currentRequestContext, IFileStorageService fileStorageService, IUnitOfWork unitOfWork)
    {
        _vehicleImageRepository = vehicleImageRepository;
        _currentRequestContext = currentRequestContext;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultReponse<bool>> Handle(
        DeleteVehicleImageCommand request,
        CancellationToken cancellationToken)
    {
        VehicleImageResponse? imageToDelete = await _vehicleImageRepository.GetImageByIdAsync(request.TenantId, request.VehicleId, request.ImageId, cancellationToken);

        if (imageToDelete is null) throw new ApiException($"Vehicle image with ID {request.ImageId} not found.", StatusCodes.Status404NotFound);

        await _vehicleImageRepository.DeleteImageAsync(request.TenantId, request.VehicleId, request.ImageId, _currentRequestContext.ModifiedBy, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await _fileStorageService.DeleteAsync(imageToDelete.PublicId, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ApiException($"Vehicle image delete failed: {ex.Message}", StatusCodes.Status502BadGateway);
        }

        return ResultReponse<bool>.Success(true);
    }
}
