using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.RentCars.Commands.CreateRentCar;
using Rentify.Backend.Core.Application.Modules.RentCars.Commands.UpdateRentCar;
using Rentify.Backend.Core.Application.Modules.RentCars.Commands.UploadRentCarLogo;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.RentCars.Contracts.Services;
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
    
    public async Task<Guid> CreateRentCarAsync(CreateRentCarCommand rentCarCommand,CancellationToken cancellationToken = default)
    {
        if(await _rentCarRepository.ValidateEmailExistAsync(rentCarCommand.ContactInformation.Email))
            throw new ApiException($"Rent car with Email '{rentCarCommand.ContactInformation.Email}' already exists.",StatusCodes.Status400BadRequest);

        if(await _rentCarRepository.ValidatePhoneNumberExistAsync(rentCarCommand.ContactInformation.PhoneNumber))
            throw new ApiException($"Rent car with Phone Number '{rentCarCommand.ContactInformation.PhoneNumber}' already exists.",StatusCodes.Status400BadRequest);

        if(await _rentCarRepository.ValidateWhatsAppExistAsync(rentCarCommand.ContactInformation.WhatsApp))
            throw new ApiException($"Rent car with WhatsApp '{rentCarCommand.ContactInformation.WhatsApp}' already exists.",StatusCodes.Status400BadRequest);

        RentCar rentCar = RentCar.Create
        (rentCarCommand.Name,
            rentCarCommand.Description,
            rentCarCommand.ContactInformation.PhoneNumber,
            rentCarCommand.ContactInformation.Email,
            rentCarCommand.ContactInformation.WhatsApp,
            rentCarCommand.AddressInformation.Street,
            rentCarCommand.AddressInformation.City,
            rentCarCommand.AddressInformation.Country,
            rentCarCommand.LogoUrl,
            rentCarCommand.CreatedBy,
            rentCarCommand.TenantId
            );
        
        await _rentCarRepository.AddAsync(
            rentCar,
            cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return rentCar.Id;
    }

    public async Task<Guid> UpdateRentCarAsync(UpdateRentCarCommand rentCarCommand, CancellationToken cancellationToken = default)
    {
        RentCar rentCar = await _rentCarRepository.GetByIdAsync(rentCarCommand.Id, cancellationToken)
                          ?? throw new ApiException("Rent car profile not found.", StatusCodes.Status404NotFound);

        if (await _rentCarRepository.ValidateEmailExistAsync(
                rentCarCommand.ContactInformation.Email,
                cancellationToken,
                rentCar.Id))
            throw new ApiException($"Rent car with Email '{rentCarCommand.ContactInformation.Email}' already exists.", StatusCodes.Status400BadRequest);

        if (await _rentCarRepository.ValidatePhoneNumberExistAsync(
                rentCarCommand.ContactInformation.PhoneNumber,
                cancellationToken,
                rentCar.Id))
            throw new ApiException($"Rent car with Phone Number '{rentCarCommand.ContactInformation.PhoneNumber}' already exists.", StatusCodes.Status400BadRequest);

        if (await _rentCarRepository.ValidateWhatsAppExistAsync(
                rentCarCommand.ContactInformation.WhatsApp,
                cancellationToken,
                rentCar.Id))
            throw new ApiException($"Rent car with WhatsApp '{rentCarCommand.ContactInformation.WhatsApp}' already exists.", StatusCodes.Status400BadRequest);

        rentCar.Update(
            rentCarCommand.Name,
            rentCarCommand.Description,
            rentCarCommand.ContactInformation.PhoneNumber,
            rentCarCommand.ContactInformation.Email,
            rentCarCommand.ContactInformation.WhatsApp,
            rentCarCommand.AddressInformation.Street,
            rentCarCommand.AddressInformation.City,
            rentCarCommand.AddressInformation.Country,
            rentCarCommand.LogoUrl,
            rentCarCommand.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return rentCar.Id;
    }

    public async Task<string> UploadLogoAsync(UploadRentCarLogoCommand command, CancellationToken cancellationToken = default)
    {
        RentCar rentCar = await _rentCarRepository.GetByIdAsync(command.RentCarId, cancellationToken)
                          ?? throw new ApiException("Rent car profile not found.", StatusCodes.Status404NotFound);

        string? previousLogoUrl = rentCar.LogoUrl;
        StoredImageResult storedLogo = await _imageStorageService.SaveRentCarLogoAsync(
            rentCar.Id,
            command.Logo,
            cancellationToken);

        rentCar.UpdateLogo(storedLogo.Url, command.ModifiedBy);

        if (!string.IsNullOrWhiteSpace(previousLogoUrl))
            await _imageStorageService.DeleteAsync(previousLogoUrl, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return storedLogo.Url;
    }
}
