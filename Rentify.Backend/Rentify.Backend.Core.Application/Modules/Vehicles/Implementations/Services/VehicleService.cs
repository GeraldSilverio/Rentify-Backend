using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Storage;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Tenants.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.SetPrimaryVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Vehicles.Dtos;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Implementations.Services;

public sealed class VehicleService : IVehicleService
{
    private const string VehicleImagesFolder = "vehicles";
    private const int MaxImagesPerVehicle = 5;

    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleCatalogRepository _vehicleCatalogRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ITenantAccessService _tenantAccessService;
    private readonly ICurrentSubscriptionService _currentSubscriptionService;
    private readonly ITenantUsageService _tenantUsageService;
    private readonly IUnitOfWork _unitOfWork;

    public VehicleService(
        IVehicleRepository vehicleRepository,
        IVehicleCatalogRepository vehicleCatalogRepository,
        IFileStorageService fileStorageService,
        ITenantAccessService tenantAccessService,
        ICurrentSubscriptionService currentSubscriptionService,
        ITenantUsageService tenantUsageService,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _vehicleCatalogRepository = vehicleCatalogRepository;
        _fileStorageService = fileStorageService;
        _tenantAccessService = tenantAccessService;
        _currentSubscriptionService = currentSubscriptionService;
        _tenantUsageService = tenantUsageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateVehicleResponse> CreateAsync(CreateVehicleCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureTenantCanUseVehiclesAsync(command.TenantId, cancellationToken);
        await _tenantUsageService.EnsureCanCreateVehicleAsync(command.TenantId, cancellationToken);
        await EnsureCatalogsCanBeUsedAsync(
            command.VehicleBrandId,
            command.VehicleModelId,
            command.VehicleTypeId,
            cancellationToken);

        await EnsureFeaturesCanBeUsedAsync(command.FeatureIds, cancellationToken);

        if (await _vehicleRepository.PlateNumberExistsAsync(command.TenantId, command.PlateNumber, cancellationToken: cancellationToken))
            throw new ApiException($"Vehicle with plate number '{command.PlateNumber}' already exists for this tenant.", StatusCodes.Status400BadRequest);

        if (await _vehicleRepository.VinExistsAsync(command.TenantId, command.Vin, cancellationToken: cancellationToken))
            throw new ApiException($"Vehicle with VIN '{command.Vin}' already exists for this tenant.", StatusCodes.Status400BadRequest);

        Vehicle vehicle = Vehicle.Create(
            command.TenantId,
            command.VehicleBrandId,
            command.VehicleModelId,
            command.VehicleTypeId,
            command.Year,
            command.PlateNumber,
            command.Vin,
            command.Color,
            command.CurrentMileage,
            command.CreatedBy);

        vehicle.ReplaceRates(
            command.Rates
                .Select(rate => (rate.RentalType, rate.Price))
                .ToArray(),
            command.CreatedBy);

        if (command.FeatureIds.Count > 0)
            vehicle.ReplaceFeatures(command.FeatureIds, command.CreatedBy);

        await _vehicleRepository.AddAsync(vehicle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateVehicleResponse(
            vehicle.Id,
            vehicle.TenantId,
            vehicle.VehicleBrandId,
            vehicle.VehicleModelId,
            vehicle.VehicleTypeId,
            vehicle.Year,
            vehicle.PlateNumber,
            vehicle.Vin,
            vehicle.Color,
            vehicle.CurrentMileage,
            vehicle.Rates
                .Where(rate => !rate.IsDeleted)
                .OrderBy(rate => rate.RentalType)
                .Select(rate => new CreateVehicleRateResponse(rate.RentalType, rate.Price))
                .ToList(),
            vehicle.FeatureAssignments
                .Where(feature => !feature.IsDeleted)
                .Select(feature => feature.VehicleFeatureId)
                .ToList(),
            vehicle.Status,
            vehicle.IsActive,
            vehicle.CreatedDate);
    }

    public async Task UpdateAsync(UpdateVehicleCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        if (await _vehicleRepository.PlateNumberExistsAsync(command.TenantId, command.PlateNumber, command.VehicleId, cancellationToken))
            throw new ApiException($"Vehicle with plate number '{command.PlateNumber}' already exists for this tenant.", StatusCodes.Status400BadRequest);

        if (await _vehicleRepository.VinExistsAsync(command.TenantId, command.Vin, command.VehicleId, cancellationToken))
            throw new ApiException($"Vehicle with VIN '{command.Vin}' already exists for this tenant.", StatusCodes.Status400BadRequest);

        vehicle.Update(
            command.VehicleBrandId,
            command.VehicleModelId,
            command.VehicleTypeId,
            command.Year,
            command.PlateNumber,
            command.Vin,
            command.Color,
            command.CurrentMileage,
            command.ModifiedBy);

        vehicle.ReplaceRates(
            [(RentalType.Daily, command.DailyRate)],
            command.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeleteVehicleCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        vehicle.Delete(command.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<VehicleImageResponse>> UploadImagesAsync(
        UploadVehicleImageCommand command,
        CancellationToken cancellationToken = default)
    {
        await EnsureTenantCanUseVehiclesAsync(command.TenantId, cancellationToken);

        Vehicle vehicle = await GetVehicleWithImagesOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        int existingImagesCount = vehicle.Images.Count(image => !image.IsDeleted);
        if (existingImagesCount + command.Images.Count > MaxImagesPerVehicle)
        {
            throw new ApiException(
                $"A vehicle can have a maximum of {MaxImagesPerVehicle} images.",
                StatusCodes.Status400BadRequest);
        }

        var uploadedFiles = new List<StoredFileResult>();

        try
        {
            foreach (IFormFile file in command.Images)
            {
                uploadedFiles.Add(await _fileStorageService.UploadAsync(file, VehicleImagesFolder, cancellationToken));
            }
        }
        catch (Exception ex)
        {
            await DeleteUploadedFilesAsync(uploadedFiles);
            throw new ApiException($"Vehicle image upload failed: {ex.Message}", StatusCodes.Status502BadGateway);
        }

        var images = new List<VehicleImage>(uploadedFiles.Count);

        for (int index = 0; index < uploadedFiles.Count; index++)
        {
            StoredFileResult storedFile = uploadedFiles[index];

            // A batch has one primary intent. The first new image receives it;
            // Vehicle.AddImage preserves the invariant for the full collection.
            bool isPrimary = command.IsPrimary && index == 0;
            images.Add(vehicle.AddImage(storedFile.Url, storedFile.PublicId, isPrimary, command.CreatedBy));
        }

        // The aggregate is tracked and owns the relationship; AddRange makes the
        // state of the new children explicit without attaching or updating the vehicle.
        await _vehicleRepository.AddImagesAsync(images, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            await DeleteUploadedFilesAsync(uploadedFiles);
            throw new ApiException(
                "Vehicle was changed or deleted while its images were being uploaded. Please retry.",
                StatusCodes.Status409Conflict);
        }
        catch
        {
            await DeleteUploadedFilesAsync(uploadedFiles);
            throw;
        }

        return images
            .Select(ToImageResponse)
            .ToList();
    }

    public async Task<IReadOnlyCollection<VehicleImageResponse>> GetImagesAsync(
        Guid tenantId,
        Guid vehicleId,
        CancellationToken cancellationToken = default)
    {
        await EnsureTenantCanUseVehiclesAsync(tenantId, cancellationToken);

        Vehicle vehicle = await GetVehicleWithImagesOrThrowAsync(tenantId, vehicleId, cancellationToken);

        return vehicle.Images
            .Where(image => !image.IsDeleted)
            .OrderByDescending(image => image.IsPrimary)
            .ThenBy(image => image.CreatedDate)
            .Select(ToImageResponse)
            .ToList();
    }

    public async Task DeleteImageAsync(
        Guid tenantId,
        Guid vehicleId,
        Guid imageId,
        string modifiedBy,
        CancellationToken cancellationToken = default)
    {
        await EnsureTenantCanUseVehiclesAsync(tenantId, cancellationToken);

        Vehicle vehicle = await GetVehicleWithImagesOrThrowAsync(tenantId, vehicleId, cancellationToken);
        VehicleImage image = vehicle.Images.FirstOrDefault(x => x.Id == imageId && !x.IsDeleted)
                             ?? throw new ApiException("Vehicle image not found.", StatusCodes.Status404NotFound);

        string publicId = image.PublicId;
        vehicle.RemoveImage(imageId, modifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await _fileStorageService.DeleteAsync(publicId, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ApiException($"Vehicle image delete failed: {ex.Message}", StatusCodes.Status502BadGateway);
        }
    }

    public async Task SetPrimaryImageAsync(
        SetPrimaryVehicleImageCommand command,
        CancellationToken cancellationToken = default)
    {
        await EnsureTenantCanUseVehiclesAsync(command.TenantId, cancellationToken);

        Vehicle vehicle = await GetVehicleWithImagesOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        try
        {
            vehicle.SetPrimaryImage(command.ImageId, command.ModifiedBy);
        }
        catch (ArgumentException ex)
        {
            throw new ApiException(ex.Message, StatusCodes.Status404NotFound);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<VehicleFeatureResponse>> GetFeaturesAsync(
        Guid tenantId,
        Guid vehicleId,
        CancellationToken cancellationToken = default)
    {
        await EnsureTenantCanUseVehiclesAsync(tenantId, cancellationToken);

        Vehicle vehicle = await _vehicleRepository.GetByIdWithFeaturesAsync(tenantId, vehicleId, cancellationToken)
                          ?? throw new ApiException("Vehicle not found.", StatusCodes.Status404NotFound);

        return vehicle.FeatureAssignments
            .Where(x => !x.IsDeleted && x.VehicleFeature is { IsDeleted: false })
            .OrderBy(x => x.VehicleFeature.Category)
            .ThenBy(x => x.VehicleFeature.Name)
            .Select(x => new VehicleFeatureResponse(
                x.VehicleFeature.Id,
                x.VehicleFeature.Name,
                x.VehicleFeature.Category,
                x.VehicleFeature.IsActive))
            .ToList();
    }

    public async Task ReplaceFeaturesAsync(
        Guid tenantId,
        Guid vehicleId,
        IReadOnlyCollection<Guid> featureIds,
        string modifiedBy,
        CancellationToken cancellationToken = default)
    {
        await EnsureTenantCanUseVehiclesAsync(tenantId, cancellationToken);

        Guid[] distinctFeatureIds = featureIds.Distinct().ToArray();
        if (distinctFeatureIds.Length != featureIds.Count)
            throw new ApiException("Vehicle features cannot contain duplicated values.", StatusCodes.Status400BadRequest);

        IReadOnlyCollection<Guid> activeFeatureIds = await _vehicleCatalogRepository.GetActiveFeatureIdsAsync(
            distinctFeatureIds,
            cancellationToken);

        if (activeFeatureIds.Count != distinctFeatureIds.Length)
            throw new ApiException("One or more vehicle features do not exist or are inactive.", StatusCodes.Status400BadRequest);

        Vehicle vehicle = await _vehicleRepository.GetByIdWithFeaturesAsync(tenantId, vehicleId, cancellationToken)
                          ?? throw new ApiException("Vehicle not found.", StatusCodes.Status404NotFound);

        vehicle.ReplaceFeatures(distinctFeatureIds, modifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(ChangeVehicleStatusCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        vehicle.ChangeStatus(command.Status, command.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task BlockAvailabilityAsync(BlockVehicleAvailabilityCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle vehicle = await GetVehicleOrThrowAsync(command.TenantId, command.VehicleId, cancellationToken);

        try
        {
            vehicle.AddUnavailableDate(command.StartDate, command.EndDate, command.Reason, command.CreatedBy);
        }
        catch (ArgumentException ex)
        {
            throw new ApiException(ex.Message, StatusCodes.Status400BadRequest);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Vehicle> GetVehicleOrThrowAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _vehicleRepository.GetByIdAsync(tenantId, vehicleId, cancellationToken)
               ?? throw new ApiException("Vehicle not found.", StatusCodes.Status404NotFound);
    }

    private async Task EnsureTenantCanUseVehiclesAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        await _tenantAccessService.EnsureTenantIsActiveAsync(tenantId, cancellationToken);
        _ = await _currentSubscriptionService.GetCurrentSubscriptionAsync(tenantId, cancellationToken);
    }

    private async Task EnsureCatalogsCanBeUsedAsync(
        Guid vehicleBrandId,
        Guid vehicleModelId,
        Guid vehicleTypeId,
        CancellationToken cancellationToken)
    {
        VehicleBrand brand = await _vehicleCatalogRepository.GetBrandByIdAsync(vehicleBrandId, cancellationToken)
                             ?? throw new ApiException("Vehicle brand not found.", StatusCodes.Status400BadRequest);

        if (!brand.IsActive)
            throw new ApiException("Vehicle brand is inactive.", StatusCodes.Status400BadRequest);

        VehicleModel model = await _vehicleCatalogRepository.GetModelByIdAsync(vehicleModelId, cancellationToken)
                             ?? throw new ApiException("Vehicle model not found.", StatusCodes.Status400BadRequest);

        if (!model.IsActive)
            throw new ApiException("Vehicle model is inactive.", StatusCodes.Status400BadRequest);

        if (model.VehicleBrandId != vehicleBrandId)
            throw new ApiException("Vehicle model does not belong to the selected brand.", StatusCodes.Status400BadRequest);

        VehicleType type = await _vehicleCatalogRepository.GetTypeByIdAsync(vehicleTypeId, cancellationToken)
                           ?? throw new ApiException("Vehicle type not found.", StatusCodes.Status400BadRequest);

        if (!type.IsActive)
            throw new ApiException("Vehicle type is inactive.", StatusCodes.Status400BadRequest);
    }

    private async Task EnsureFeaturesCanBeUsedAsync(
        IReadOnlyCollection<Guid> featureIds,
        CancellationToken cancellationToken)
    {
        if (featureIds.Count == 0)
            return;

        Guid[] distinctFeatureIds = featureIds.Distinct().ToArray();
        if (distinctFeatureIds.Length != featureIds.Count)
            throw new ApiException("Vehicle features cannot contain duplicated values.", StatusCodes.Status400BadRequest);

        IReadOnlyCollection<Guid> activeFeatureIds = await _vehicleCatalogRepository.GetActiveFeatureIdsAsync(
            distinctFeatureIds,
            cancellationToken);

        if (activeFeatureIds.Count != distinctFeatureIds.Length)
            throw new ApiException("One or more vehicle features do not exist or are inactive.", StatusCodes.Status400BadRequest);
    }

    private async Task<Vehicle> GetVehicleWithImagesOrThrowAsync(Guid tenantId, Guid vehicleId, CancellationToken cancellationToken)
    {
        return await _vehicleRepository.GetByIdWithImagesAsync(tenantId, vehicleId, cancellationToken)
               ?? throw new ApiException("Vehicle not found.", StatusCodes.Status404NotFound);
    }
    private async Task DeleteUploadedFilesAsync(IEnumerable<StoredFileResult> uploadedFiles)
    {
        foreach (StoredFileResult uploadedFile in uploadedFiles)
        {
            try
            {
                await _fileStorageService.DeleteAsync(uploadedFile.PublicId, CancellationToken.None);
            }
            catch
            {
                // Preserve the original failure; cleanup can be retried operationally.
            }
        }
    }

    private static VehicleImageResponse ToImageResponse(VehicleImage image)
    {
        return new VehicleImageResponse(
            image.Id,
            image.Url,
            image.PublicId,
            image.IsPrimary,
            image.CreatedDate);
    }
}
