using MediatR;
using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Helpers;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.Storage;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Repositories;
using Rentify.Backend.Core.Domain.Entities.Vehicles;

namespace Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;

public sealed class UploadVehicleImageHandler
    : IRequestHandler<UploadVehicleImageCommand, ResultReponse<IReadOnlyCollection<VehicleImageResponse>>>
{
    private const int MaxConcurrentUploads = 3;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IVehicleImageRepository _vehicleImageRepository;
    private readonly int _maxImagesPerVehicle;
    private readonly string _vehicleImagesFolder;

    public UploadVehicleImageHandler(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IVehicleImageRepository vehicleImageRepository)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _vehicleImageRepository = vehicleImageRepository;
        _maxImagesPerVehicle = int.Parse(ReadFromConfiguration.GetValueFromConfig("MaxImagesPerVehicle"));
        _vehicleImagesFolder = ReadFromConfiguration.GetValueFromConfig("VehicleImagesFolder");
    }

    public async Task<ResultReponse<IReadOnlyCollection<VehicleImageResponse>>> Handle(
        UploadVehicleImageCommand command,
        CancellationToken cancellationToken)
    {
        ValidateImages(command.Images);

        int existingImagesCount = await _vehicleImageRepository.GetTotalImagesByVehicleAsync(command.TenantId, command.VehicleId, cancellationToken);

        if (existingImagesCount + command.Images.Count > _maxImagesPerVehicle)
        {
            throw new ApiException(
                $"A vehicle can have a maximum of {_maxImagesPerVehicle} images.",
                StatusCodes.Status400BadRequest);
        }

        List<StoredFileResult> uploadedFiles = [];

        try
        {
            uploadedFiles = await UploadFilesWithLimitPreservingOrderAsync(
                command.Images,
                _vehicleImagesFolder,
                MaxConcurrentUploads,
                cancellationToken);

            var images = new List<VehicleImage>(uploadedFiles.Count);

        for (int index = 0; index < uploadedFiles.Count; index++)
        {
            StoredFileResult storedFile = uploadedFiles[index];
            bool isPrimary = command.IsPrimary && index == 0;

            //Si el vehiculo no tiene imagenes y es el primer elemento, colocarle primary.
                isPrimary = existingImagesCount == 0 && index == 0;

                VehicleImage image = VehicleImage.Create(command.TenantId,command.VehicleId,
                    storedFile.Url,
                    storedFile.PublicId,
                    isPrimary,
                    command.CreatedBy);

                images.Add(image);
            }

            await _vehicleImageRepository.AddImagesAsync(images,cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            IReadOnlyCollection<VehicleImageResponse> response = images
                .Select(image => new VehicleImageResponse(
                    image.Id,
                    image.Url,
                    image.PublicId,
                    image.IsPrimary,
                    image.CreatedDate))
                .ToList();

            return ResultReponse<IReadOnlyCollection<VehicleImageResponse>>.Success(response);
        }
        catch (ConcurrencyException)
        {
            await DeleteUploadedFilesSafelyAsync(uploadedFiles, CancellationToken.None);

            throw new ApiException(
                "Vehicle was changed while images were being uploaded. Please retry.",
                StatusCodes.Status409Conflict);
        }
        catch
        {
            await DeleteUploadedFilesSafelyAsync(uploadedFiles, CancellationToken.None);
            throw;
        }
    }

    private async Task<List<StoredFileResult>> UploadFilesWithLimitPreservingOrderAsync(
        IReadOnlyCollection<IFormFile> files,
        string folder,
        int maxConcurrency,
        CancellationToken cancellationToken)
    {
        using var semaphore = new SemaphoreSlim(maxConcurrency);
        IFormFile[] filesToUpload = files.ToArray();
        var uploadedFiles = new StoredFileResult?[filesToUpload.Length];

        Task[] tasks = filesToUpload
            .Select((file, index) => UploadFileAsync(file, index))
            .ToArray();

        try
        {
            await Task.WhenAll(tasks);
        }
        catch
        {
            IReadOnlyCollection<StoredFileResult> completedUploads = uploadedFiles
                .Where(file => file is not null)
                .Select(file => file!)
                .ToList();

            await DeleteUploadedFilesSafelyAsync(completedUploads, CancellationToken.None);
            throw;
        }

        return uploadedFiles
            .Select(file => file ?? throw new InvalidOperationException("Vehicle image upload did not complete."))
            .ToList();

        async Task UploadFileAsync(IFormFile file, int index)
        {
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                uploadedFiles[index] = await _fileStorageService.UploadAsync(
                    file,
                    folder,
                    cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

    private async Task DeleteUploadedFilesSafelyAsync(
        IReadOnlyCollection<StoredFileResult> uploadedFiles,
        CancellationToken cancellationToken)
    {
        if (uploadedFiles.Count == 0)
            return;

        IEnumerable<Task> deleteTasks = uploadedFiles
            .Where(file => !string.IsNullOrWhiteSpace(file.PublicId))
            .Select(file => _fileStorageService.DeleteAsync(file.PublicId, cancellationToken));

        try
        {
            await Task.WhenAll(deleteTasks);
        }
        catch
        {
        }
    }

    private static void ValidateImages(IReadOnlyCollection<IFormFile> images)
    {
        if (images is null || images.Count == 0)
        {
            throw new ApiException(
                "At least one image is required.",
                StatusCodes.Status400BadRequest);
        }

        foreach (IFormFile image in images)
        {
            if (image.Length == 0)
            {
                throw new ApiException(
                    "One or more images are empty.",
                    StatusCodes.Status400BadRequest);
            }

            if (image.Length > 5 * 1024 * 1024)
            {
                throw new ApiException(
                    "Each image must be 5 MB or less.",
                    StatusCodes.Status400BadRequest);
            }

            string contentType = image.ContentType.ToLowerInvariant();

            if (contentType is not ("image/jpeg" or "image/jpg" or "image/png" or "image/webp"))
            {
                throw new ApiException(
                    "Only JPG, PNG and WEBP images are allowed.",
                    StatusCodes.Status400BadRequest);
            }
        }
    }
}
