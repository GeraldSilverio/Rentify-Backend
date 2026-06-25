using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.RentCars.Commands.UpdateRentCar;
using Rentify.Backend.Core.Application.Modules.RentCars.Commands.UploadRentCarLogo;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.RentCars.Dtos;
using Rentify.Backend.Core.Application.Modules.Vehicles.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Application.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Implementations.Services;

public sealed class RentCarService : IRentCarService
{
    private readonly IRentCarRepository _rentCarRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IUnitOfWork _unitOfWork;
    
    public RentCarService(
        IRentCarRepository rentCarRepository,
        IImageStorageService imageStorageService,
        IUnitOfWork unitOfWork)
    {
        _rentCarRepository = rentCarRepository;
        _imageStorageService = imageStorageService;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Guid> CreateRentCarAsync(CreateRentCarDto createRentCar,CancellationToken cancellationToken = default)
    {
        if(await _rentCarRepository.ValidateEmailExistAsync(createRentCar.ContactInformation.Email))
            throw new ApiException($"El email:'{createRentCar.ContactInformation.Email}' ya existe.",StatusCodes.Status400BadRequest);

        if(await _rentCarRepository.ValidatePhoneNumberExistAsync(createRentCar.ContactInformation.PhoneNumber))
            throw new ApiException($"El número de teléfono '{createRentCar.ContactInformation.PhoneNumber}' ya existe.",StatusCodes.Status400BadRequest);

        if(await _rentCarRepository.ValidateWhatsAppExistAsync(createRentCar.ContactInformation.WhatsApp))
            throw new ApiException($"El WhatsApp '{createRentCar.ContactInformation.WhatsApp}' ya existe.",StatusCodes.Status400BadRequest);

        RentCar rentCar = RentCar.Create
        (createRentCar.Name,
            createRentCar.Description,
            createRentCar.ContactInformation.PhoneNumber,
            createRentCar.ContactInformation.Email,
            createRentCar.ContactInformation.WhatsApp,
            createRentCar.AddressInformation.Street,
            createRentCar.AddressInformation.City,
            createRentCar.AddressInformation.Country,
            "",
            createRentCar.CreatedBy,
            createRentCar.TenantId
            );
        
        await _rentCarRepository.AddAsync(
            rentCar,
            cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return rentCar.Id;
    }

    public async Task<Guid> UpdateRentCarAsync(UpdateRentCarCommand createRentCar, CancellationToken cancellationToken = default)
    {
        RentCar rentCar = await _rentCarRepository.GetByIdAsync(createRentCar.Id, cancellationToken)
                          ?? throw new ApiException("Rent car profile not found.", StatusCodes.Status404NotFound);

        if (await _rentCarRepository.ValidateEmailExistAsync(
                createRentCar.ContactInformation.Email,
                cancellationToken,
                rentCar.Id))
            throw new ApiException($"Rent car with Email '{createRentCar.ContactInformation.Email}' already exists.", StatusCodes.Status400BadRequest);

        if (await _rentCarRepository.ValidatePhoneNumberExistAsync(
                createRentCar.ContactInformation.PhoneNumber,
                cancellationToken,
                rentCar.Id))
            throw new ApiException($"Rent car with Phone Number '{createRentCar.ContactInformation.PhoneNumber}' already exists.", StatusCodes.Status400BadRequest);

        if (await _rentCarRepository.ValidateWhatsAppExistAsync(
                createRentCar.ContactInformation.WhatsApp,
                cancellationToken,
                rentCar.Id))
            throw new ApiException($"Rent car with WhatsApp '{createRentCar.ContactInformation.WhatsApp}' already exists.", StatusCodes.Status400BadRequest);

        rentCar.Update(
            createRentCar.Name,
            createRentCar.Description,
            createRentCar.ContactInformation.PhoneNumber,
            createRentCar.ContactInformation.Email,
            createRentCar.ContactInformation.WhatsApp,
            createRentCar.AddressInformation.Street,
            createRentCar.AddressInformation.City,
            createRentCar.AddressInformation.Country,
            createRentCar.LogoUrl,
            createRentCar.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return rentCar.Id;
    }

    public async Task<string> UploadLogoAsync(UploadRentCarLogoCommand command, CancellationToken cancellationToken = default)
    {
        RentCar rentCar = await _rentCarRepository.GetByIdAsync(command.RentCarId, cancellationToken)
                          ?? throw new ApiException("Rent car profile not found.", StatusCodes.Status404NotFound);

        string? previousLogoPublicId = rentCar.LogoPublicId;
        StoredImageResult storedLogo = await _imageStorageService.UploadRentCarLogoAsync(
            command.Logo,
            cancellationToken);

        rentCar.UpdateLogo(storedLogo.Url, storedLogo.PublicId, command.ModifiedBy);

        if (!string.IsNullOrWhiteSpace(previousLogoPublicId))
            await _imageStorageService.DeleteAsync(previousLogoPublicId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return storedLogo.Url;
    }
}
